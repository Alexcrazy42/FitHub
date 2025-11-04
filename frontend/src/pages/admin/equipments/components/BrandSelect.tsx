import { Select, Spin } from "antd";
import { Control, Controller } from "react-hook-form";
import { useState, useCallback, useEffect } from "react";
import debounce from "lodash.debounce";
import { ListResponse } from "../../../../types/common";
import { IBrandResponse } from "../../../../types/equipments";
import { useApiService } from "../../../../api/useApiService";

export const BrandSelect: React.FC<{
  control: Control;
  name: string;
  initialLabel?: string;
}> = ({ control, name, initialLabel }) => {
  const [options, setOptions] = useState<{ label: string; value: string }[]>([]);
  const [fetching, setFetching] = useState(false);
  const apiService = useApiService();

  useEffect(() => {
    const initialValue = (control as any)._formValues?.[name];
    if (initialLabel && initialValue) {
      setOptions([{ label: initialLabel, value: initialValue }]);
    }
  }, [initialLabel, control, name]);

  const fetchBrands = useCallback(
    debounce(async (search: string) => {
      if (search.length < 3) return;
      setFetching(true);
      try {
        const response = await apiService.get<ListResponse<IBrandResponse>>(
          `/v1/brands?Name=${search}&PageNumber=1&PageSize=10`
        );
        if (response.success) {
          const newOptions = (response.data?.items || []).map((b: IBrandResponse) => ({
            label: b.name,
            value: b.id,
          }));

          setOptions(() => {
            
            const unique = newOptions.filter(
              (item, index, self) =>
                index === self.findIndex((o) => o.value === item.value)
            );
            return unique;
          });
        }
      } catch {
        // игнорируем ошибки
      } finally {
        setFetching(false);
      }
    }, 500),
    []
  );

  return (
    <Controller
      name={name}
      control={control}
      render={({ field, fieldState }) => {
        return (
          <>
            <Select
              showSearch
              placeholder="Выберите бренд"
              notFoundContent={fetching ? <Spin size="small" /> : null}
              filterOption={false}
              options={options}
              onSearch={fetchBrands}
              onChange={(val) => field.onChange(val)}
              value={field.value}
              labelInValue={false}
            />
            {fieldState.error && (
              <p className="text-red-500 text-sm mt-1">{fieldState.error.message}</p>
            )}
          </>
        );
      }}
    />
  );
};
