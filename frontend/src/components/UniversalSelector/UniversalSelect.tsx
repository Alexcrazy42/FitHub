import { Select, AutoComplete, Spin } from 'antd';
import type { SelectProps } from 'antd';
import React, { useEffect, useMemo, useState, useRef } from 'react';

// ===================== ТИПЫ =====================

export type OptionType = {
  value: string | number;
  label: string;
  payload?: unknown;
};

type Mode = 'static' | 'prefetch' | 'infinite' | 'autocomplete';

type BaseProps = {
  mode: Mode;
  value?: string | number | (string | number)[];
  onChange?: (value: unknown, option?: unknown) => void;
  placeholder?: string;
  allowClear?: boolean;
  multiple?: boolean;
  disabled?: boolean;
  style?: React.CSSProperties;
  className?: string;
};

type StaticProps = BaseProps & {
  mode: 'static';
  options: OptionType[];
};

type PrefetchProps = BaseProps & {
  mode: 'prefetch';
  fetchAll: () => Promise<OptionType[]>;
};

type InfiniteProps = BaseProps & {
  mode: 'infinite';
  pageSize?: number;
  fetchPage: (page: number, pageSize: number) => Promise<{
    items: OptionType[];
    hasMore: boolean;
  }>;
};

type AutocompleteProps = BaseProps & {
  mode: 'autocomplete';
  minSearchLength?: number;
  debounceMs?: number;
  fetchSearch: (query: string) => Promise<OptionType[]>;
};

export type UniversalSelectProps =
  | StaticProps
  | PrefetchProps
  | InfiniteProps
  | AutocompleteProps;

export const UniversalSelect: React.FC<UniversalSelectProps> = (props : UniversalSelectProps) => {
  const {
    value,
    onChange,
    placeholder,
    allowClear = true,
    multiple,
    disabled,
    style,
    className,
  } = props;

  const [options, setOptions] = useState<OptionType[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (props.mode === 'static') {
      setOptions(props.options);
    }
  }, [props]);

  const isPrefetch = props.mode === 'prefetch';

  useEffect(() => {
    if (!isPrefetch) return;
    
    setLoading(true);
    props
      .fetchAll()
      .then(setOptions)
      .catch((error) => {
        console.error('Error fetching options:', error);
        setOptions([]);
      })
      .finally(() => setLoading(false));
  }, [isPrefetch, props]);

  const isInfinite = props.mode === 'infinite';
  const [page, setPage] = useState(1);
  const [hasMore, setHasMore] = useState(true);
  
  const isLoadingRef = useRef<boolean>(false);
  const scrollDebounceRef = useRef<NodeJS.Timeout>(undefined);

  const loadPage = async (nextPage: number) => {
    if (!isInfinite || !hasMore || isLoadingRef.current) {
      return;
    }

    isLoadingRef.current = true;
    setLoading(true);

    try {
      const result = await props.fetchPage(nextPage, props.pageSize ?? 20);
      
      setOptions((prev) => {
        // Фильтруем дубликаты
        const existingIds = new Set(prev.map((p) => p.value));
        const newItems = result.items.filter(
          (item) => !existingIds.has(item.value)
        );
        return [...prev, ...newItems];
      });
      
      setHasMore(result.hasMore);
      setPage(nextPage);
    } catch (error) {
      console.error('Error loading page:', error);
    } finally {
      setLoading(false);
      isLoadingRef.current = false;
    }
  };

  useEffect(() => {
    if (!isInfinite) return;
    
    // Сбрасываем состояние и загружаем первую страницу
    setOptions([]);
    setPage(1);
    setHasMore(true);
    loadPage(1);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isInfinite]);

  const handlePopupScroll: SelectProps['onPopupScroll'] = (e) => {
    if (!isInfinite || !hasMore || isLoadingRef.current) return;

    // Очищаем предыдущий таймаут
    if (scrollDebounceRef.current) {
      clearTimeout(scrollDebounceRef.current);
    }

    scrollDebounceRef.current = setTimeout(() => {
      const target = e.target as HTMLDivElement;
      const threshold = 50; // Запас в пикселях до конца списка
      
      // Проверяем, достигли ли низа
      const isBottom = 
        target.scrollTop + target.offsetHeight >= target.scrollHeight - threshold;

      if (isBottom) {
        loadPage(page + 1);
      }
    }, 150); // Debounce 150ms
  };

  // Очистка таймаута при размонтировании
  useEffect(() => {
    return () => {
      if (scrollDebounceRef.current) {
        clearTimeout(scrollDebounceRef.current);
      }
    };
  }, []);

  // ===== 4) AUTOCOMPLETE MODE (поиск) =====
  const isAutocomplete = props.mode === 'autocomplete';
  const [searchValue, setSearchValue] = useState('');
  const searchDebounceRef = useRef<NodeJS.Timeout>(undefined);

  const minSearchLength = isAutocomplete ? props.minSearchLength ?? 2 : 0;
  const debounceMs = isAutocomplete ? props.debounceMs ?? 300 : 0;

  const handleSearch = (query: string) => {
    if (!isAutocomplete) return;
    
    setSearchValue(query);

    // Очищаем предыдущий таймаут
    if (searchDebounceRef.current) {
      clearTimeout(searchDebounceRef.current);
    }

    // Если запрос слишком короткий, очищаем опции
    if (query.length < minSearchLength) {
      setOptions([]);
      return;
    }

    // Debounce для поиска
    searchDebounceRef.current = setTimeout(() => {
      setLoading(true);
      props
        .fetchSearch(query)
        .then(setOptions)
        .catch((error) => {
          console.error('Error searching:', error);
          setOptions([]);
        })
        .finally(() => setLoading(false));
    }, debounceMs);
  };

  // Очистка таймаута поиска при размонтировании
  useEffect(() => {
    return () => {
      if (searchDebounceRef.current) {
        clearTimeout(searchDebounceRef.current);
      }
    };
  }, []);

  // ===== ОБЩИЕ НАСТРОЙКИ =====
  const selectOptions: SelectProps['options'] = useMemo(
    () =>
      options.map((o) => ({
        value: o.value,
        label: o.label,
      })),
    [options],
  );

  const commonSelectProps: SelectProps = {
    value,
    onChange,
    mode: multiple ? 'multiple' : undefined,
    placeholder,
    allowClear,
    disabled,
    className,
    style: { width: '100%', ...style },
    options: selectOptions,
    loading,
    notFoundContent: loading ? <Spin size="small" /> : null,
  };

  // ===== РЕНДЕР =====
  
  // Режим автокомплита
  if (isAutocomplete) {
    return (
      <AutoComplete
        {...commonSelectProps}
        onSearch={handleSearch}
        value={searchValue}
        placeholder={placeholder ?? 'Начните вводить...'}
      />
    );
  }

  // Остальные режимы (static, prefetch, infinite)
  return (
    <Select
      {...commonSelectProps}
      showSearch
      onPopupScroll={handlePopupScroll}
      filterOption={(input, option) =>
        (option?.label as string)
          ?.toLowerCase()
          .includes(input.toLowerCase())
      }
      virtual={isInfinite || options.length > 100}
    />
  );
};
