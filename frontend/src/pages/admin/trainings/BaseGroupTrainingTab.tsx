import { useEffect, useState } from "react";
import { IBaseGroupTraining, ICreateBaseGroupTraining, ITrainingType } from "../../../types/trainings";
import { useApiService } from "../../../api/useApiService";
import { ListResponse } from "../../../types/common";
import { Table, Pagination, Modal, Button, Input, InputNumber, Switch, Select, Space, Tag } from "antd";
import type { ColumnsType } from "antd/es/table";
import { useForm, Controller } from "react-hook-form";
import { ValidationError } from "../../../api/ApiService";
import { toast } from "react-toastify";

interface BaseGroupTrainingTabProps {
  activeTab: string;
}

export const BaseGroupTrainingTab: React.FC<BaseGroupTrainingTabProps> = ({ activeTab }) => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [total, setTotal] = useState(0);
  const [baseGroupTrainings, setBaseGroupTrainings] = useState<IBaseGroupTraining[]>([]);
  const [loading, setLoading] = useState(false);
  const [modalVisible, setModalVisible] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [trainingTypesOptions, setTrainingTypesOptions] = useState<ITrainingType[]>([]);

  const apiService = useApiService();

  const { control, handleSubmit, reset, setError, clearErrors } = useForm<ICreateBaseGroupTraining>({
    defaultValues: {
      name: "",
      description: "",
      durationInMinutes: 30,
      complexity: 1,
      isActive: true,
      trainingTypeIds: [],
    },
  });

  const fetch = async () => {
    setLoading(true);
    try {
      const response = await apiService.get<ListResponse<IBaseGroupTraining>>(
        `/v1/base-group-trainings?PageNumber=${page}&PageSize=${pageSize}`
      );
      if (response.success) {
        setBaseGroupTrainings(response.data?.items ?? []);
        setTotal(response.data?.totalItems ?? 0);
      }
    } catch {
      toast.error("Неизвестная ошибка при загрузке списка");
    } finally {
      setLoading(false);
    }
  };

  const fetchTrainingTypes = async () => {
    try {
      const resp = await apiService.get<ListResponse<ITrainingType>>(`/v1/training-types`);
      if (resp.success) {
        setTrainingTypesOptions(resp.data?.items ?? []);
      } else {
        // не фатально
        console.warn("Не удалось получить training types: ", resp.error);
      }
    } catch (e) {
      console.warn(e);
    }
  };

  
  useEffect(() => {
    if (activeTab === "trainings") {
      fetch();
      fetchTrainingTypes();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [activeTab, page, pageSize]);

  function mapServerValidationErrors(errors?: ValidationError[]) {
    if (!errors || errors.length === 0) return;

    const mapPropertyToField = (propertyName: string) : string => {
      if (!propertyName) return propertyName;
      // простая трансформация: первая буква в нижний регистр
      return propertyName.charAt(0).toLowerCase() + propertyName.slice(1);
    };

    for (const err of errors) {
      const field = mapPropertyToField(err.propertyName);
      if (!field) continue;

      // Если поле есть в форме — устанавливаем ошибку, иначе кладём глобальную
      setError(field as any, { type: "server", message: err.message });
    }
  }


  const openCreateModal = () => {
    clearErrors();
    reset();
    setModalVisible(true);
  };

  const onCreateSubmit = async (payload: ICreateBaseGroupTraining) => {
    setSubmitting(true);
    try {
      const request: ICreateBaseGroupTraining = {
        name: payload.name,
        description: payload.description,
        durationInMinutes: payload.durationInMinutes,
        complexity: payload.complexity,
        isActive: payload.isActive,
        trainingTypeIds: payload.trainingTypeIds,
      };

      const resp = await apiService.post<IBaseGroupTraining>("/v1/base-group-trainings", request);
      if (resp.success) {
        toast.success("Группа тренировок создана");
        setModalVisible(false);
        fetch();
      } else {
        const problem = resp.error;
        if (problem?.errors && problem.errors.length) {
          mapServerValidationErrors(problem.errors);
          toast.error(
            <div dangerouslySetInnerHTML={{ __html: problem?.detail.replace(/\n/g, "<br />") }} />
            );
        } else {
          toast.error(problem?.detail ?? "Ошибка создания");
        }
      }
    } catch {
      toast.error("Неизвестная ошибка при создании");
    } finally {
      setSubmitting(false);
    }
  };

  const columns: ColumnsType<IBaseGroupTraining> = [
    {
      title: "Название",
      dataIndex: "name",
      key: "name",
      render: (text) => text ?? "-",
    },
    {
      title: "Описание",
      dataIndex: "description",
      key: "description",
      render: (text) => text ?? "-",
    },
    {
      title: "Продолжительность (мин)",
      dataIndex: "durationInMinutes",
      key: "durationInMinutes",
      render: (v) => v ?? "-",
    },
    {
      title: "Сложность",
      dataIndex: "complexity",
      key: "complexity",
      render: (v) => v ?? "-",
    },
    {
      title: "Активно",
      dataIndex: "isActive",
      key: "isActive",
      render: (v) => (v ? <Tag color="green">Активно</Tag> : <Tag color="error">Неактивно</Tag>),
    },
    {
      title: "Типы тренировок",
      dataIndex: "trainingTypes",
      key: "trainingTypes",
      render: (types: ITrainingType[]) => (
        <div className="flex flex-wrap gap-2">
          {types?.map((t) => (
            <span key={t.id} className="px-2 py-1 text-sm border rounded">
              {t.name}
            </span>
          ))}
        </div>
      ),
    },
  ];

  return (
    <div className="p-4">
      <Space className="mb-4">
        <Button type="primary" onClick={openCreateModal}>
          Создать группу тренировок
        </Button>
      </Space>

      <Table<IBaseGroupTraining>
        rowKey={(r) => r.id ?? ""}
        dataSource={baseGroupTrainings}
        columns={columns}
        loading={loading}
        pagination={false}
      />

      <div className="flex justify-end mt-4">
        <Pagination
          current={page}
          pageSize={pageSize}
          total={total}
          onChange={(p, ps) => {
            setPage(p);
            setPageSize(ps);
          }}
        />
      </div>

      <Modal
        title="Создать группу тренировок"
        visible={modalVisible}
        onCancel={() => setModalVisible(false)}
        footer={null}
        destroyOnClose
      >
        <form onSubmit={handleSubmit(onCreateSubmit)} className="space-y-3">
          <div>
            <label className="block mb-1">Название</label>
            <Controller
              control={control}
              name="name"
            //   rules={{ required: "Обязательно!", minLength: { value: 3, message: "Минимум 3 символа" } }}
              render={({ field, fieldState }) => (
                <>
                  <Input {...field} />
                  {fieldState.error && (
                    <div className="text-red-600 mt-1 text-sm">{fieldState.error.message}</div>
                  )}
                </>
              )}
            />
          </div>

          <div>
            <label className="block mb-1">Описание</label>
            <Controller
              control={control}
              name="description"
              render={({ field }) => <Input.TextArea {...field} rows={3} />}
            />
          </div>

          <div>
            <label className="block mb-1">Продолжительность в минутах</label>
            <Controller
              control={control}
              name="durationInMinutes"
            //   rules={{ min: { value: 1, message: "Должно быть >= 1" } }}
              render={({ field, fieldState }) => (
                <>
                  <InputNumber {...field} min={1} style={{ width: "100%" }} />
                  {fieldState.error && <div className="text-red-600 mt-1 text-sm">{fieldState.error.message}</div>}
                </>
              )}
            />
          </div>

          <div>
            <label className="block mb-1">Сложность</label>
            <Controller
              control={control}
              name="complexity"
              rules={{ min: { value: 1, message: "Минимум 1" }, max: { value: 3, message: "Максимум 3" } }}
              render={({ field, fieldState }) => (
                <>
                  <InputNumber {...field}  style={{ width: "100%" }} /> 
                  {fieldState.error && <div className="text-red-600 mt-1 text-sm">{fieldState.error.message}</div>}
                </>
              )}
            />
          </div>

          <div className="flex items-center gap-4">
            <label>Активно</label>
            <Controller
              control={control}
              name="isActive"
              render={({ field }) => <Switch checked={field.value} onChange={field.onChange} />}
            />
          </div>

          <div>
            <label className="block mb-1">Типы тренировок</label>
            <Controller
                control={control}
                name="trainingTypeIds"
                render={({ field, fieldState }) => (
                <>
                    <Select
                    mode="multiple"
                    allowClear
                    className="w-full"
                    style={{ minWidth: 280 }}
                    value={field.value}
                    onChange={field.onChange}
                    options={trainingTypesOptions.map((t) => ({
                        label: t.name,
                        value: t.id,
                    }))}
                    />
                    {fieldState.error && (
                    <div className="text-red-600 mt-1 text-sm">
                        {fieldState.error.message}
                    </div>
                    )}
                </>
                )}
            />
            </div>

          <div className="flex justify-end gap-2">
            <Button onClick={() => setModalVisible(false)}>Отмена</Button>
            <Button type="primary" htmlType="submit" loading={submitting}>
              Создать
            </Button>
          </div>
        </form>
      </Modal>
    </div>
  );
};
