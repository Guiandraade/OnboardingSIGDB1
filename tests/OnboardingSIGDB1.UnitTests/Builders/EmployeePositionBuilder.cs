using System.Reflection;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.UnitTests.Builders;

public class EmployeePositionBuilder
{
    private Employee? _employee;
    private Position? _position;
    private DateTime _startDate = DateTime.UtcNow.AddDays(-1);
    private int? _employeeId;
    private int? _positionId;

    public static EmployeePositionBuilder New() => new();

    public EmployeePositionBuilder WithEmployee(Employee employee) { _employee = employee; return this; }
    public EmployeePositionBuilder WithPosition(Position position) { _position = position; return this; }
    public EmployeePositionBuilder WithStartDate(DateTime startDate) { _startDate = startDate; return this; }
    public EmployeePositionBuilder WithEmployeeId(int id) { _employeeId = id; return this; }
    public EmployeePositionBuilder WithPositionId(int id) { _positionId = id; return this; }

    public EmployeePosition Build()
    {
        var employee = _employee ?? EmployeeBuilder.New().Build();
        var position = _position ?? PositionBuilder.New().Build();

        if (_employeeId.HasValue) SetId(employee, _employeeId.Value);
        if (_positionId.HasValue) SetId(position, _positionId.Value);

        return new EmployeePosition(employee, position, _startDate);
    }

    internal static void SetId(object obj, int id)
    {
        var type = obj.GetType();
        var prop = type.GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop != null)
        {
            var setMethod = prop.GetSetMethod(true);
            if (setMethod != null)
            {
                setMethod.Invoke(obj, new object[] { id });
                return;
            }
        }

        var field = type.GetField("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(obj, id);
            return;
        }

        throw new InvalidOperationException($"Could not set Id on type {type.FullName}");
    }
}

