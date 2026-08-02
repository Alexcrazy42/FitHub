from typing import Generic, Optional, TypeVar, List
from pydantic import Field, BaseModel

T = TypeVar('T')

class ListResponse(BaseModel, Generic[T]):
    """Generic модель для списковых ответов"""
    items: List[T] = Field(default_factory=list)
    currentPage: Optional[int] = Field(default=1, ge=1)
    pageSize: Optional[int] = Field(default=10, ge=1, le=100)
    totalItems: Optional[int] = Field(default=0, ge=0)
    totalPages: Optional[int] = Field(default=0, ge=0)
    
    @property
    def has_next(self) -> bool:
        """Есть ли следующая страница"""
        return self.currentPage < self.totalPages
    
    @property
    def has_previous(self) -> bool:
        """Есть ли предыдущая страница"""
        return self.currentPage > 1

