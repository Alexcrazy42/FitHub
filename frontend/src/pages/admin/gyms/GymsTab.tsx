import React, { useEffect, useState } from "react";
import {
  Table,
  Pagination,
  Drawer,
  Button,
  Space,
  Modal,
  Input,
  Form as AntForm,
} from "antd";
import { toast } from "react-toastify";
import { useApiService } from "../../../api/useApiService";
import {
  IGymResponse,
  IUpdateGymRequest,
  ICreateGymRequest,
} from "../../../types/gyms";
import { ListResponse } from "../../../types/common";
import { GymForm } from "./GymForm";
import { useForm, Controller } from "react-hook-form";

interface GymsTabProps {
  activeTab: string;
}

export const GymsTab: React.FC<GymsTabProps> = ({ activeTab }) => {
  const apiService = useApiService();

  const [loading, setLoading] = useState(false);
  const [gyms, setGyms] = useState<IGymResponse[]>([]);
  const [selectedGym, setSelectedGym] = useState<IGymResponse | null>(null);
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [formLoading, setFormLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalItems, setTotalItems] = useState(0);

  // состояния модалки
  const [createModalVisible, setCreateModalVisible] = useState(false);

  // react-hook-form
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ICreateGymRequest>({
    defaultValues: {
      name: "",
      description: "",
    },
  });

  const fetchGyms = async () => {
    try {
      setLoading(true);
      const res = await apiService.get<ListResponse<IGymResponse>>(
        `v1/gyms?PageNumber=${currentPage}&PageSize=${pageSize}`
      );
      if (res.success && res.data) {
        setGyms(res.data.items);
        const total = res.data.totalItems;
        if (total) setTotalItems(total);
      }
    } catch {
      toast.error("Ошибка при загрузке спортзалов");
    } finally {
      setLoading(false);
    }
  };

  const getGym = async (gym: IGymResponse): Promise<IGymResponse> => {
    const res = await apiService.get<IGymResponse>(`v1/gyms/${gym.id}`);
    if (res.success && res.data) return res.data;
    return gym;
  };

  const refreshGym = async (gym: IGymResponse) => {
    const fresh = await getGym(gym);
    setSelectedGym(fresh);
    return fresh;
  };

  

  const handleSave = async (values: IUpdateGymRequest): Promise<IGymResponse> => {
    try {
      setFormLoading(true);
      const res = await apiService.put<IGymResponse>(`v1/gyms`, values);
      if (res.success && res.data) {
        toast.success("Спортзал успешно обновлён");
        fetchGyms();
        setDrawerVisible(false);
        return res.data;
      }
      throw new Error("Ошибка обновления");
    } catch {
      toast.error("Ошибка при сохранении спортзала");
      throw new Error("Ошибка сохранения");
    } finally {
      setFormLoading(false);
    }
  };

  const handleCreate = async (values: ICreateGymRequest) => {
    try {
      setFormLoading(true);
      const res = await apiService.post<IGymResponse>(`v1/gyms`, values);
      if (res.success && res.data) {
        toast.success("Спортзал успешно создан");
        await fetchGyms();
        setCreateModalVisible(false);
        reset();

        // Загружаем свежий зал и открываем Drawer
        const newGym = await getGym(res.data);
        setSelectedGym(newGym);
        setDrawerVisible(true);
      } else {
        toast.error(res.error?.detail || "Ошибка при создании спортзала");
      }
    } catch {
      toast.error("Ошибка при создании спортзала");
    } finally {
      setFormLoading(false);
    }
  };

  const handleRowClick = async (gym: IGymResponse) => {
    const currentGym = await getGym(gym);
    setSelectedGym(currentGym);
    setDrawerVisible(true);
  };

  useEffect(() => {
    if(activeTab === "gyms") {
      fetchGyms();
    }
  }, [activeTab, currentPage, pageSize]);

  return (
    <>
      {/* Верхняя панель действий */}
      <div className="flex justify-between mb-4">
        <Space>
          <Button type="primary" onClick={fetchGyms}>
            Обновить
          </Button>
          <Button type="default" onClick={() => setCreateModalVisible(true)}>
            Добавить зал
          </Button>
        </Space>
      </div>

      {/* Таблица */}
      <Table
        columns={[
          { title: "Название", dataIndex: "name", key: "name" },
          { title: "Описание", dataIndex: "description", key: "description" },
        ]}
        dataSource={gyms}
        rowKey="id"
        loading={loading}
        pagination={false}
        onRow={(record) => ({
          onClick: () => handleRowClick(record),
          style: { cursor: "pointer" },
        })}
      />

      {/* Пагинация */}
      <div className="flex justify-end mt-4">
        <Pagination
          current={currentPage}
          pageSize={pageSize}
          total={totalItems}
          onChange={(p, s) => {
            setCurrentPage(p);
            setPageSize(s);
          }}
          showSizeChanger
          showQuickJumper
          showTotal={(total, range) =>
            `Показано ${range[0]}-${range[1]} из ${total} записей`
          }
          pageSizeOptions={["10", "20", "50", "100"]}
        />
      </div>

      {/* Drawer для редактирования */}
      <Drawer
        title={`Редактирование зала: ${selectedGym?.name}`}
        placement="right"
        onClose={() => setDrawerVisible(false)}
        open={drawerVisible}
        width={600}
        destroyOnClose
      >
        {selectedGym && (
          <GymForm
            gym={selectedGym}
            onSave={handleSave}
            onCancel={() => setDrawerVisible(false)}
            loading={formLoading}
            refresh={refreshGym}
            refreshAll={fetchGyms}
          />
        )}
      </Drawer>

      {/* Modal для создания нового зала */}
      <Modal
        title="Создание нового зала"
        open={createModalVisible}
        onCancel={() => {
          setCreateModalVisible(false);
          reset();
        }}
        onOk={handleSubmit(handleCreate)}
        confirmLoading={formLoading}
        okText="Создать"
        cancelText="Отмена"
      >
        <AntForm layout="vertical">
          <AntForm.Item
            label="Название"
            validateStatus={errors.name ? "error" : ""}
            help={errors.name?.message}
          >
            <Controller
              name="name"
              control={control}
              rules={{ required: "Введите название" }}
              render={({ field }) => (
                <Input {...field} placeholder="Например: Iron Gym" />
              )}
            />
          </AntForm.Item>

          <AntForm.Item
            label="Описание"
            validateStatus={errors.description ? "error" : ""}
            help={errors.description?.message}
          >
            <Controller
              name="description"
              control={control}
              rules={{ required: "Введите описание" }}
              render={({ field }) => (
                <Input.TextArea {...field} rows={3} placeholder="Краткое описание..." />
              )}
            />
          </AntForm.Item>
        </AntForm>
      </Modal>
    </>
  );
};
