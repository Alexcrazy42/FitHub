import React, { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Card,
  Typography,
  Descriptions,
  message,
  Button,
  Form,
  Input,
  Switch,
  DatePicker,
  Space,
  Tag,
  Skeleton,
  Empty,
  Carousel
} from "antd";
import { EditOutlined, SaveOutlined, CloseOutlined, ArrowLeftOutlined, RightOutlined, LeftOutlined } from "@ant-design/icons";
import { useForm, Controller } from "react-hook-form";
import {
  IEquipmentResponse,
  ICreateEquipmentRequest,
} from "../../../types/equipments";
import { useApiService } from "../../../api/useApiService";
import dayjs from "dayjs";
import { toast } from "react-toastify";
import { BrandSelect } from "./components/BrandSelect";
import ImageUploader, { ImageUploaderHandle } from "../../../components/ImageUploader/ImageUploader";

const { Title } = Typography;

export const EquipmentPage: React.FC = () => {
  const { equipmentId } = useParams<{ equipmentId: string }>();
  const apiService = useApiService();
  const navigate = useNavigate();
  const carouselRef = useRef<unknown>(null);
  const [equipment, setEquipment] = useState<IEquipmentResponse | null | undefined>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [isEditing, setIsEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const uploaderRef = useRef<ImageUploaderHandle>(null);
  const { control, handleSubmit, reset } = useForm<ICreateEquipmentRequest>();
  const [maxFileCount, setMaxFileCount] = useState(1);
  const [isWaitingToConfirmUpload, setIsWaitingToConfirmUpload] = useState<boolean>(false);
  const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);


  const fileUploadedFromUploader = async (fileId: string): Promise<void> => {
    setUploadedFileId(fileId);
  };

  const fetchEquipment = async () => {
    try {
      setLoading(true);
      const response = await apiService.get<IEquipmentResponse>(
        `/v1/equipments/${equipmentId}`
      );
      if (response.success) {
        setEquipment(response?.data);
        reset({
          brandId: response.data?.brand.id,
          name: response.data?.name,
          description: response.data?.description,
          additionalDescroption: response.data?.additionalDescroption || "",
          instructionAddBefore: response.data?.instructionAddBefore || null,
          isActive: response.data?.isActive,
        });
      }
    } catch {
      message.error("Не удалось загрузить данные о тренажёре");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (equipmentId) fetchEquipment();
  }, [equipmentId]);

  const onSubmit = async (data: ICreateEquipmentRequest) => {
    try {
      setSaving(true);
      const payload: ICreateEquipmentRequest = {
        ...data,
        instructionAddBefore: data.instructionAddBefore
      };

      const response = await apiService.put<IEquipmentResponse>(
        `/v1/equipments/${equipmentId}`,
        payload
      );

      if (response.success) {
        toast.success("Данные успешно обновлены!");
        setIsEditing(false);
        await fetchEquipment();
      } else {
        toast.error(response.error?.detail || "Ошибка при обновлении");
      }
    } catch {
      toast.error("Ошибка при сохранении данных");
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="p-6 flex justify-center">
        <Card className="w-full max-w-3xl shadow-lg rounded-2xl">
          <Skeleton active paragraph={{ rows: 6 }} />
        </Card>
      </div>
    );
  }

  if (!equipment) {
    return (
      <div className="flex justify-center items-center h-[60vh]">
        <Card
          className="w-full max-w-md text-center shadow-lg rounded-2xl p-6"
          bordered={false}
        >
          <Empty
            description={
              <span className="text-gray-600 text-lg">
                Тренажёр не найден
              </span>
            }
          />
          <Button
            type="primary"
            className="mt-4"
            onClick={() => navigate("/admin/equipments")}
          >
            Вернуться к списку
          </Button>
        </Card>
      </div>
    );
  }

  return (
    <div className="p-6 flex justify-center">
    <Card className="w-full max-w-3xl shadow-lg rounded-2xl">
      <div className="flex justify-between items-center mb-4">
        <div className="flex items-center gap-4">
          {!isEditing && (
            <Button
              type="default"
              icon={<ArrowLeftOutlined />}
              onClick={() => navigate("/admin/equipments")}
            >
              Назад к списку
            </Button>
          )}
          
          <Title level={3} className="m-0">
            {isEditing ? "Редактирование тренажёра" : equipment.name}
          </Title>
        </div>

        {isEditing ? (
          <Space>
            <Button
              icon={<CloseOutlined />}
              onClick={() => {
                reset({
                  brandId: equipment.brand.id,
                  name: equipment.name,
                  description: equipment.description,
                  additionalDescroption: equipment.additionalDescroption || "",
                  instructionAddBefore: equipment.instructionAddBefore || null,
                  isActive: equipment.isActive,
                });
                setIsEditing(false);
              }}
            >
              Отмена
            </Button>
            <Button
              type="primary"
              icon={<SaveOutlined />}
              loading={saving}
              onClick={handleSubmit(onSubmit)}
            >
              Сохранить
            </Button>
          </Space>
        ) : (
          <Button
            type="primary"
            icon={<EditOutlined />}
            onClick={() => setIsEditing(true)}
          >
            Редактировать
          </Button>
        )}
      </div>
          {isEditing ? (
            <Form layout="vertical" onFinish={handleSubmit(onSubmit)}>
              <Form.Item label="Бренд" required>
                <BrandSelect
                  control={control}
                  name="brandId"
                  initialLabel={equipment.brand.name}
                />
              </Form.Item>

              <Form.Item label="Название" required>
                <Controller
                  name="name"
                  control={control}
                  rules={{ required: "Введите название" }}
                  render={({ field, fieldState }) => (
                    <>
                      <Input {...field} value={field.value ?? ""} placeholder="Название оборудования" />
                      {fieldState.error && (
                        <p className="text-red-500 text-sm mt-1">
                          {fieldState.error.message}
                        </p>
                      )}
                    </>
                  )}
                />
              </Form.Item>

              <Form.Item label="Описание">
                <Controller
                  name="description"
                  control={control}
                  render={({ field }) => (
                    <Input.TextArea
                      {...field}
                      value={field.value ?? ""}
                      placeholder="Описание"
                      rows={3}
                    />
                  )}
                />
              </Form.Item>

              <Form.Item label="Доп. описание">
                <Controller
                  name="additionalDescroption"
                  control={control}
                  render={({ field }) => (
                    <Input.TextArea
                      {...field}
                      value={field.value ?? ""}
                      placeholder="Доп. описание"
                      rows={2}
                    />
                  )}
                />
              </Form.Item>

              <Form.Item label="Инструкция до">
                <Controller
                  name="instructionAddBefore"
                  control={control}
                  render={({ field }) => (
                    <DatePicker
                      {...field}
                      format="DD-MM-YYYY"
                      value={field.value ? dayjs(field.value) : null}
                      onChange={(date) =>
                        field.onChange(date ? date.format("YYYY-MM-DD") : null)
                      }
                    />
                  )}
                />
              </Form.Item>

              <Form.Item label="Активно">
                <Controller
                  name="isActive"
                  control={control}
                  render={({ field }) => (
                    <Switch
                      checked={!!field.value}
                      onChange={(checked) => field.onChange(checked)}
                    />
                  )}
                />
              </Form.Item>
            </Form>
          ) : (
            <>
            <Descriptions bordered column={1}>
              <Descriptions.Item label="Описание">
                {equipment.description || "—"}
              </Descriptions.Item>
              <Descriptions.Item label="Доп. описание">
                {equipment.additionalDescroption || "—"}
              </Descriptions.Item>
              <Descriptions.Item label="Инструкция будет добавлена до">
                {equipment.instructionAddBefore
                  ? new Date(
                      equipment.instructionAddBefore
                    ).toLocaleDateString()
                  : "—"}
              </Descriptions.Item>
              <Descriptions.Item label="Активен">
                {equipment.isActive ? (
                  <Tag color="green">Активно</Tag>
                ) : (
                  <Tag color="red">Неактивно</Tag>
                )}
              </Descriptions.Item>
              <Descriptions.Item label="Бренд">
                <strong>
                  {equipment.brand?.name ? (
                    <Tag>{equipment.brand.name}</Tag>
                  ) : (
                    "—"
                  )}
                </strong>
              </Descriptions.Item>
            </Descriptions>

            
              <Title level={4}>Фотографии оборудования</Title>
              
              <div className="relative w-full max-w-3xl mx-auto">
              <div className="relative">
                <Carousel ref={carouselRef}>
                  {Array.from({ length: 5 }).map((_, index) => (
                    <div key={index} className="flex justify-center">
                      <img
                        src="http://localhost:5209/api/v1/files/0199f6ea-ee4d-7740-8e92-15a67df28913"
                        alt={`Фото ${index + 1}`}
                        style={{ maxHeight: "400px", objectFit: "contain", width: "100%" }}
                      />
                    </div>
                  ))}
                </Carousel>

                {/* Кнопка Назад */}
                <Button
                  type="primary"
                  shape="circle"
                  icon={<LeftOutlined />}
                  onClick={() => carouselRef.current.prev()}
                  className="absolute top-1/2 -translate-y-1/2 -left-4 z-10"
                >
                  
                </Button>

                {/* Кнопка Вперед */}
                <Button
                  type="primary"
                  shape="circle"
                  icon={<RightOutlined />}
                  onClick={() => carouselRef.current.next()}
                  className="absolute top-1/2 -translate-y-1/2 -right-4 z-10"
                />
              </div>
            </div>

            <ImageUploader ref={uploaderRef} maxFileCount={maxFileCount} fileUpload={fileUploadedFromUploader} onSuccessCancel={()=>{setIsWaitingToConfirmUpload(true)}} />
            
            </>
          )}
        </Card>
    </div>
  );
};
