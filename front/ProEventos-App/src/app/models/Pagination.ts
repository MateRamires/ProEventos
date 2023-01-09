export class Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

export class PaginatedResult<T> {
  result: T; //O T seria qualquer coisa, ou seja o result pode ser uma lista de evento, lote, palestrante, etc.
  pagination: Pagination;
}
