import React, { useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Card,
  Typography,
  Descriptions,
  Button,
  Form,
  Input,
  Switch,
  DatePicker,
  Space,
  Tag,
  Skeleton,
  Empty,
  Carousel,
  Modal
} from "antd";
import { EditOutlined, SaveOutlined, CloseOutlined, ArrowLeftOutlined, RightOutlined, LeftOutlined, DeleteOutlined } from "@ant-design/icons";
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
import { CarouselRef } from "antd/es/carousel";
import { EntityType, IEntity, IFileResponse, IMakeFilesActiveRequest } from "../../../types/files";
import { getFileRoute } from "../../../api/files";
import { ListResponse } from "../../../types/common";
const { Title } = Typography;



export const EquipmentPage: React.FC = () => {
  const { equipmentId } = useParams<{ equipmentId: string }>();
  const apiService = useApiService();
  const navigate = useNavigate();
  const carouselRef = useRef<CarouselRef>(null);
  const [equipment, setEquipment] = useState<IEquipmentResponse | null | undefined>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [isEditing, setIsEditing] = useState(false);
  const [saving, setSaving] = useState(false);
  const uploaderRef = useRef<ImageUploaderHandle>(null);
  const { control, handleSubmit, reset } = useForm<ICreateEquipmentRequest>();
  const [maxFileCount, setMaxFileCount] = useState(1);
  const [isWaitingToConfirmUpload, setIsWaitingToConfirmUpload] = useState<boolean>(false);
  const [uploadedFileId, setUploadedFileId] = useState<string | null>(null);
  const [files, setFiles] = useState<IFileResponse[]>([]);
  const [fileCount, setFileCount] = useState(0);


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
      toast.error("Не удалось загрузить данные о тренажёре");
    } finally {
      setLoading(false);
    }
  };

  const fetchMaxFileCount = async () => {
    const response = await apiService.get<IEntity>(`/v1/entities?entityType=${EntityType.Equipment}`);
    if(response.success) {
      const maxCount = response.data?.maxFileCount;
      if(maxCount) {
        setMaxFileCount(maxCount);
      }
    }
  }

  const fetchFiles = async () => {
    if(equipmentId) {
      const response = await apiService.get<ListResponse<IFileResponse>>(`/v1/files?entityId=${equipmentId}&entityType=${EntityType.Equipment}`);
      if(response.success) {
        setFiles(response.data?.items ?? []);
      }
    }
  }

  useEffect(() => {
    if (equipmentId){
      fetchEquipment();
      fetchMaxFileCount();
      fetchFiles();
    } 
  }, [equipmentId]);

  useEffect(() => {
    if (files.length > fileCount && carouselRef.current) {
      carouselRef.current.goTo(files.length - 1);
    }
    setFileCount(files.length);
  }, [files.length]);

  const handleAttachImage = async () => {
      if (!uploadedFileId) return;
      try {
        const makeFilesActiveRequest : IMakeFilesActiveRequest = {
          fileIds : [uploadedFileId],
          entityId: equipment.id,
          entityType: EntityType.Equipment
        }
        const response = await apiService.post(`/v1/files/make-files-active`, makeFilesActiveRequest);
        if(response.success) {
          toast.success("Изображение успешно привязано к тренажеру!");
          setUploadedFileId(null);
          setIsWaitingToConfirmUpload(false);
          await fetchFiles();
          setTimeout(() => {
            if (carouselRef.current) {
                carouselRef.current.goTo(files.length);
              }
            }, 100);
        } else {
          toast.error("Не получилось привязать изображение!")
          setUploadedFileId(null);
          setIsWaitingToConfirmUpload(false);
        }
      } catch (error) {
        console.error(error);
        toast.error("Не получилось привязать изображение!")
      }
  };

  const handleAddPhotoClick = () => {
    uploaderRef.current?.openFileDialog();
  };

  const onSubmit = async (data: ICreateEquipmentRequest) => {
    try {
      setSaving(true);

      const response = await apiService.put<IEquipmentResponse>(
        `/v1/equipments/${equipmentId}`,
        data
      );

      if (response.success) {
        toast.success("Данные успешно обновлены!");
        await fetchEquipment();
        setIsEditing(false);
      } else {
        toast.error(response.error?.detail || "Ошибка при обновлении");
      }
    } catch {
      toast.error("Ошибка при сохранении данных");
    } finally {
      setSaving(false);
    }
  };

  const handleRemoveFile = async (fileId : string) => {
    const response = await apiService.delete(`v1/files/${fileId}`);
    if(response.success) {
      await fetchFiles();
      toast.success("Файл успешно удален!");
    } else {
      toast.error("Ошибка при удалении файла!")
    }
  }

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
                  additionalDescroption: equipment.additionalDescroption,
                  instructionAddBefore: equipment.instructionAddBefore,
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

            
            <Title level={4}>Фотографии оборудования {files.length} шт.</Title>
            
            <div className="relative w-full max-w-3xl mx-auto">
              <div className="relative">
                <div className="absolute top-2 right-2 z-20">
                  <Button type="primary" onClick={handleAddPhotoClick}>
                    Добавить фото
                  </Button>
                </div>
                <Carousel ref={carouselRef} dots={false}>
                  {files.map((file, index) => (
                    <div key={index} className="relative flex flex-col items-center">
                      <img
                        src={getFileRoute(file.id)}
                        alt={file.fileName}
                        style={{ maxHeight: "400px", objectFit: "contain", width: "100%" }}
                      />
                      
                      <div className="mt-2 text-center text-lg font-medium block w-full">
                        <span className="text-center text-lg font-medium">
                          {file.fileName}
                        </span>
                        <Button
                          type="link"
                          danger
                          icon={<DeleteOutlined />}
                          onClick={(e) => {
                            e.preventDefault();
                            e.stopPropagation();
                            handleRemoveFile(file.id)
                          }}
                        />
                      </div>
                    </div>
                  ))}
                </Carousel>

                {files.length > 0 && (
                  <div className="flex justify-between items-center w-full px-4">
                    <Button
                      type="link"
                      shape="circle"
                      icon={<LeftOutlined />}
                      onClick={() => carouselRef.current?.prev()}
                      className="absolute top-1/2 -translate-y-1/2 -left-4 z-10"
                    >
                      
                    </Button>

                    <Button
                      type="link"
                      shape="circle"
                      icon={<RightOutlined />}
                      onClick={() => carouselRef.current?.next()}
                      className="absolute top-1/2 -translate-y-1/2 -right-4 z-10"
                    />
                  </div>
                )}
              </div>
            </div>


            {uploadedFileId && isWaitingToConfirmUpload && (
              <Modal
                open={true}
                onCancel={() => setIsWaitingToConfirmUpload(false)} // или свой метод закрытия
                footer={null}
                centered
                closable={true}
                width={400}
              >
                <div className="flex flex-col items-center gap-4">
                  <div>
                    <img
                      src={getFileRoute(uploadedFileId)}
                      alt="Тренажер"
                      className="w-64 h-64 object-cover rounded shadow-sm"
                    />
                  </div>

                  <Button type="primary" onClick={handleAttachImage}>
                    Привязать к тренажеру
                  </Button>
                </div>
              </Modal>
            )}

            <ImageUploader ref={uploaderRef} maxFileCount={maxFileCount} fileUpload={fileUploadedFromUploader} onSuccessCancel={()=>{setIsWaitingToConfirmUpload(true)}} />
            </>
          )}
        </Card>
    </div>
  );
};
