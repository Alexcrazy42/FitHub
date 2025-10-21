import { Select, Spin } from "antd";
import { Controller } from "react-hook-form";
import { useState, useCallback } from "react";
import debounce from "lodash.debounce";

export const BrandSelect: React.FC<{
  control: any;
  name: string;
  apiService: any;
}> = ({ control, name, apiService }) => {
  const [options, setOptions] = useState<{ label: string; value: string }[]>([]);
  const [fetching, setFetching] = useState(false);

  // Делаем debounce функцию для поиска
  const fetchBrands = useCallback(
    debounce(async (search: string) => {
      if (search.length < 3) return setOptions([]); // минимум 3 символа
      setFetching(true);
      try {
        const response = await apiService.get(`/v1/brands?Name=${search}&PageNumber=1&PageSize=10`);
        if (response.success) {
          setOptions(
            (response.data?.items || []).map((b: any) => ({
              label: b.name,
              value: b.id,
            }))
          );
        } else {
          setOptions([]);
        }
      } catch {
        setOptions([]);
      } finally {
        setFetching(false);
      }
    }, 500), // debounce 500ms
    []
  );

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => (
        <>
          <Select
            {...field}
            showSearch
            placeholder="Выберите бренд"
            notFoundContent={fetching ? <Spin size="small" /> : null}
            options={options}
            filterOption={false} // отключаем локальный фильтр, т.к. запрос к API
            onSearch={fetchBrands} // вызов при вводе
            onChange={(val) => field.onChange(val)}
          />
          {fieldState.error && (
            <p className="text-red-500 text-sm mt-1">{fieldState.error.message}</p>
          )}
        </>
      )}
    />
  );
};
