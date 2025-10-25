import { Select, Spin } from "antd";
import { Controller } from "react-hook-form";
import { useState, useCallback } from "react";
import debounce from "lodash.debounce";
import { ListResponse } from "../../../../types/common";
import { IBrandResponse } from "../../../../types/equipments";
import { useApiService } from "../../../../api/useApiService";

export const BrandSelect: React.FC<{
  control: any;
  name: string;
  initialLabel?: string;
}> = ({ control, name, initialLabel }) => {
  const [options, setOptions] = useState<{ label: string; value: string }[]>([]);
  const [fetching, setFetching] = useState(false);
  const apiService = useApiService();

  const fetchBrands = useCallback(
    debounce(async (search: string) => {
      if (search.length < 3) return setOptions([]);
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
          setOptions((prev) => {
            const merged = [...prev, ...newOptions];
            const unique = merged.filter(
              (item, index, self) =>
                index === self.findIndex((o) => o.value === item.value)
            );
            return unique;
          });
        } else {
          setOptions([]);
        }
      } catch {
        setOptions([]);
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
        const allOptions = [
          ...(field.value && initialLabel
            ? [{ label: initialLabel, value: field.value }]
            : []),
          ...options,
        ];

        const uniqueOptions = allOptions.filter(
          (item, index, self) =>
            index === self.findIndex((o) => o.value === item.value)
        );

        return (
          <>
            <Select
              {...field}
              showSearch
              placeholder="Выберите бренд"
              notFoundContent={fetching ? <Spin size="small" /> : null}
              options={uniqueOptions}
              filterOption={false}
              onSearch={fetchBrands}
              onChange={(val) => field.onChange(val)}
            />
            {fieldState.error && (
              <p className="text-red-500 text-sm mt-1">
                {fieldState.error.message}
              </p>
            )}
          </>
        );
      }}
    />
  );
};
