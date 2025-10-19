import React, { useState } from "react";
import { useForm, Controller } from "react-hook-form";
import { Input, Select, Button, Form, message, Modal, Space } from "antd";
import { ComplexityType, complexityTypeOptions, IEquipmentForm } from "../../../types/equipments";




const mockApiRequest = async (data: IEquipmentForm) => {
  await new Promise((r) => setTimeout(r, 800));

  if (data.name.trim().length < 3) {
    return {
      success: false,
      errors: {
        name: ["Название должно быть не короче 3 символов", "Абоба"],
        type: ["Выберите тип зала"],
      },
    };
  }
  return { success: true };
};

export const EquipmentsTab: React.FC = () => {
  const {
    control,
    handleSubmit,
    setError,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<IEquipmentForm>({
    defaultValues: {
      name: "",
      description: "",
      type: ComplexityType.Low,
    },
  });
  const [createModalVisible, setCreateModalVisible] = useState(false);

  const onSubmit = async (data: IEquipmentForm) => {
    const res = await mockApiRequest(data);

    if (!res.success && res.errors) {
      Object.entries(res.errors).forEach(([field, messages]) => {
        setError(field as keyof IEquipmentForm, {
          type: "validate",
          message: Array.isArray(messages) ? messages.join("\n") : String(messages),
        });
      });
      message.error("Исправьте ошибки в форме");
      return;
    }

    message.success("Зал успешно сохранён!");
    reset();
  };

  return (
    <>
      <div className="flex justify-between mb-4">
        <Space>
          <Button type="primary">
            Обновить
          </Button>
          <Button type="default" onClick={() => setCreateModalVisible(true)}>
            Добавить тренажер
          </Button>
        </Space>
      </div>

      <Modal
        title="Создание нового тренажера"
        open={createModalVisible}
        onCancel={() => setCreateModalVisible(false)}
        onOk={handleSubmit(onSubmit)} // 👈 при клике на "Создать" сработает submit формы
        confirmLoading={isSubmitting}
        okText="Создать"
        cancelText="Отмена"
      >
        <div className="p-6 max-w-lg">
          <Form layout="vertical" onFinish={handleSubmit(onSubmit)}>
            <Form.Item label="Название" required>
              <Controller
                name="name"
                control={control}
                rules={{ required: "Введите название" }}
                render={({ field }) => (
                  <>
                    <Input {...field} placeholder="Iron Gym" />
                    {errors.name && (
                      <span className="text-red-500 text-sm whitespace-pre-line">
                        {errors.name?.message}
                      </span>
                    )}
                  </>
                )}
              />
            </Form.Item>

            <Form.Item label="Описание" required>
              <Controller
                name="description"
                control={control}
                rules={{ required: "Введите описание" }}
                render={({ field }) => (
                  <>
                    <Input.TextArea {...field} rows={3} placeholder="Описание зала..." />
                    {errors.description && (
                      <span className="text-red-500 text-sm whitespace-pre-line">
                        {errors.description?.message}
                      </span>
                    )}
                  </>
                )}
              />
            </Form.Item>

            <Form.Item label="Тип зала" required>
              <Controller
                name="type"
                control={control}
                rules={{ required: "Выберите тип зала" }}
                render={({ field }) => (
                  <>
                    <Select {...field} options={complexityTypeOptions} placeholder="Выберите тип" />
                    {errors.type && (
                      <span className="text-red-500 text-sm whitespace-pre-line">
                        {errors.type?.message}
                      </span>
                    )}
                  </>
                )}
              />
            </Form.Item>
          </Form>
        </div>
      </Modal>
    </>
  );
};
