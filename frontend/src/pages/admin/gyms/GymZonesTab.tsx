import React, { useEffect, useState } from "react";
import { Table, Drawer, Button, Modal, Form as AntForm, Input, Space, } from "antd";
import { toast } from "react-toastify";
import { useApiService } from "../../../api/useApiService";
import { ICreateGymZoneRequest, IGymZoneResponse, IUpdateGymZoneRequest } from "../../../types/gyms";
import { ListResponse } from "../../../types/common";
import { GymZoneForm } from "./GymZoneForm";
import { Controller, useForm } from "react-hook-form";

export const GymZonesTab: React.FC = () => {
  const apiService = useApiService();
  const [loading, setLoading] = useState(false);
  const [zones, setZones] = useState<IGymZoneResponse[]>([]);
  const [selectedZone, setSelectedZone] = useState<IGymZoneResponse | null>(null);
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [formLoading, setFormLoading] = useState(false);

  // состояния модалки
  const [createModalVisible, setCreateModalVisible] = useState(false);

  const {
      control,
      handleSubmit,
      reset,
      formState: { errors },
    } = useForm<ICreateGymZoneRequest>({
      defaultValues: {
        name: "",
        description: "",
      },
    });

  const fetchZones = async () => {
    try {
      setLoading(true);
      const res = await apiService.get<ListResponse<IGymZoneResponse>>(`/v1/gym-zones`);
      if (res.success && res.data) {
        setZones(res.data.items);
      }
    } catch {
      toast.error("Ошибка при загрузке зон");
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async (values: IUpdateGymZoneRequest): Promise<IGymZoneResponse> => {
    try {
      setFormLoading(true);
      const res = await apiService.put<IGymZoneResponse>(`v1/gym-zones`, values);
      if (res.success && res.data) {
        toast.success("Зона успешно обновлена");
        fetchZones();
        setDrawerVisible(false);
        return res.data;
      }
      throw new Error("Ошибка обновления");
    } catch {
      toast.error("Ошибка при сохранении зоны");
      throw new Error("Ошибка сохранения");
    } finally {
      setFormLoading(false);
    }
  };

  const getZone = async (gymZone: IGymZoneResponse): Promise<IGymZoneResponse> => {
      const res = await apiService.get<IGymZoneResponse>(`v1/gyms-zones/${gymZone.id}`);
      if (res.success && res.data) return res.data;
      return gymZone;
  };

  const handleCreate = async (values: ICreateGymZoneRequest) => {
      try {
        setFormLoading(true);
        const res = await apiService.post<IGymZoneResponse>(`v1/gym-zones`, values);
        if (res.success && res.data) {
          toast.success("Зона успешно создана");
          await fetchZones();
          setCreateModalVisible(false);
          reset();
  
          
          const newZone = await getZone(res.data);
          setSelectedZone(newZone);
          setDrawerVisible(true);
        } else {
          toast.error(res.error?.detail || "Ошибка при создании зоны");
        }
      } catch {
        toast.error("Ошибка при создании спортзала");
      } finally {
        setFormLoading(false);
      }
    };

  const handleRowClick = (zone: IGymZoneResponse) => {
    setSelectedZone(zone);
    setDrawerVisible(true);
  };

  useEffect(() => {
    fetchZones();
  }, []);

  return (
    <>
      <div className="flex justify-between mb-4">
        <Space>
          <Button type="primary" onClick={fetchZones}>
            Обновить
          </Button>
          <Button type="default" onClick={() => setCreateModalVisible(true)}>
            Добавить зону
          </Button>
        </Space>
      </div>

      <Table
        columns={[
          { title: "Название", dataIndex: "name", key: "name" },
          { title: "Описание", dataIndex: "description", key: "description" },
        ]}
        dataSource={zones}
        rowKey="id"
        loading={loading}
        pagination={false}
        onRow={(record) => ({
          onClick: () => handleRowClick(record),
          style: { cursor: "pointer" },
        })}
      />

      <Drawer
        title={`Редактирование зоны: ${selectedZone?.name}`}
        placement="right"
        onClose={() => setDrawerVisible(false)}
        open={drawerVisible}
        width={600}
        destroyOnClose
      >
        {selectedZone && (
          <GymZoneForm
            gymZone={selectedZone}
            onSave={handleSave}
            onCancel={() => setDrawerVisible(false)}
            loading={formLoading}
            refreshAll={fetchZones}
          />
        )}
      </Drawer>

      <Modal
        title="Создание новой зоны"
        open={createModalVisible}
        onCancel={() => {
          setCreateModalVisible(false);
          reset();
        }}
        onOk={handleSubmit(handleCreate)}
        confirmLoading={formLoading}
        okText="Создать"
        cancelText="Отмена"
        destroyOnClose
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
