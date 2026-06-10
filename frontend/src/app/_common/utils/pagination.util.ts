/**
 * Utility functions for pagination display and logic.
 */

/**
 * Generate an array of page numbers for display in pagination controls.
 *
 * @param totalPages Total number of pages available
 * @param currentPage Current active page number
 * @returns Array of page numbers and ellipsis ('...') markers
 */
export function generatePageNumbers(totalPages: number, currentPage: number): (number | string)[] {
  if (totalPages <= 5) {
    return Array.from({ length: totalPages }, (_, i) => i + 1);
  }

  const pinnedPages = Array.from(new Set([1, 2, currentPage, totalPages])).sort((a, b) => a - b);
  const pageNumbers: (number | string)[] = [];

  for (let i = 0; i < pinnedPages.length; i++) {
    if (i > 0 && pinnedPages[i] - pinnedPages[i - 1] > 1) {
      pageNumbers.push('...');
    }
    pageNumbers.push(pinnedPages[i]);
  }

  return pageNumbers;
}

