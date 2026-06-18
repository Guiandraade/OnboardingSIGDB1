import { BaseFilter } from "./pagination.model";

export interface PositionRequest{
  description: string;
}

export interface PositionResponse{
  id: number;
  description: string;
}

export interface PositionFilter extends BaseFilter{
  description?: string;
}
