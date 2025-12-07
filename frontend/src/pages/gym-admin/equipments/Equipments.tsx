import React, { useState, useEffect, useRef } from "react";
import { EquipmentCondition, IAddOrUpdateGymEquipmentRequest, IEquipmentResponse, IGymEquipmentResponse, equipmentConditionsOptions } from "../../../types/equipments";
import { useApiService } from "../../../api/useApiService"
import { toast } from "react-toastify"
import { useForm, Controller } from "react-hook-form";
import { mapServerValidationErrors } from "../../../api/ApiService";
import { ListResponse } from "../../../types/common";
import { useAuth } from '../../../context/useAuth';
import { OptionType, UniversalSelect } from "../../../components/UniversalSelector/UniversalSelect";
import { 
    Card, 
    Button, 
    Table, 
    Space, 
    Tag, 
    Modal, 
    Checkbox, 
    DatePicker, 
    Select,
    Empty,
    Pagination
} from 'antd';
import { 
    PlusOutlined, 
    EditOutlined, 
    DeleteOutlined, 
    CheckCircleOutlined, 
    CloseCircleOutlined,
    ToolOutlined,
    WarningOutlined,
    HomeOutlined,
    CalendarOutlined
} from '@ant-design/icons';
import dayjs from 'dayjs';

export const Equipments: React.FC = () => {
    const apiService = useApiService();
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [total, setTotal] = useState(0);
    const [gymEquipments, setGymEquipments] = useState<IGymEquipmentResponse[]>([]);
    const [editingId, setEditingId] = useState<string | null>(null);
    const [showForm, setShowForm] = useState(false);
    const { currentGym } = useAuth();

    const formRef = useRef<HTMLDivElement>(null);

    const {
        control,
        handleSubmit,
        reset,
        setError,
        clearErrors,
        watch,
        setValue
    } = useForm<IAddOrUpdateGymEquipmentRequest>({
        defaultValues: {
            equipmentId: "",
            gymId: currentGym?.id ?? "",
            isActive: true,
            openedAt: null,
            condition: EquipmentCondition.Operational
        },
    });

    const isActive = watch("isActive");
    const condition = watch("condition");

    useEffect(() => {
        fetch();
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [page, pageSize, currentGym]);

    const fetch = async () => {
        setLoading(true);
        try {
            const params = new URLSearchParams();
            if (currentGym) {
                params.append('GymId', currentGym.id);
            }
            params.append('PageNumber', page.toString());
            params.append('PageSize', pageSize.toString());

            const response = await apiService.get<ListResponse<IGymEquipmentResponse>>(`/v1/gym-equipments?${params}`);
            if (response.success) {
                const items = response.data?.items ?? [];
                const normalized = items.map(x => ({
                    ...x,
                    openedAt: x.openedAt ? new Date(x.openedAt) : null,
                }));
                const totalItems = response.data?.totalItems ?? items.length;
                setGymEquipments(normalized);
                setTotal(totalItems);
            }
        } catch (e) {
            console.error(e);
            toast.error("Ошибка загрузки данных");
        } finally {
            setLoading(false);
        }
    }

    const create = async (req: IAddOrUpdateGymEquipmentRequest) => {
        setSubmitting(true);
        clearErrors();
        try {
            const response = await apiService.post<IGymEquipmentResponse>('/v1/gym-equipments', req);
            if (response.success) {
                toast.success("Тренажер успешно создан!");
                reset();
                setShowForm(false);
                fetch();
            } else {
                const problem = response.error;
                if (problem?.errors && problem.errors.length > 0) {
                    mapServerValidationErrors(problem.errors, setError);
                } else {
                    toast.error(problem?.detail ?? "Ошибка создания");
                }
            }
        } catch (e) {
            console.error(e);
            toast.error("Ошибка при создании!");
        } finally {
            setSubmitting(false);
        }
    }

    const update = async (id: string, req: IAddOrUpdateGymEquipmentRequest) => {
        setSubmitting(true);
        clearErrors();
        try {
            const response = await apiService.put<IGymEquipmentResponse>(`/v1/gym-equipments/${id}`, req);
            if (response.success) {
                toast.success("Тренажер успешно обновлен!");
                reset();
                setShowForm(false);
                setEditingId(null);
                fetch();
            } else {
                const problem = response.error;
                if (problem?.errors && problem.errors.length > 0) {
                    mapServerValidationErrors(problem.errors, setError);
                } else {
                    toast.error(problem?.detail ?? "Ошибка обновления");
                }
            }
        } catch (e) {
            console.error(e);
            toast.error("Ошибка при обновлении!");
        } finally {
            setSubmitting(false);
        }
    }

    const deleteEquipment = async (id: string) => {
        Modal.confirm({
            title: 'Удалить тренажер?',
            icon: <DeleteOutlined className="text-red-500" />,
            content: 'Это действие нельзя отменить',
            okText: 'Удалить',
            okType: 'danger',
            cancelText: 'Отмена',
            onOk: async () => {
                try {
                    const response = await apiService.delete(`/v1/gym-equipments/${id}`);
                    if (response.success) {
                        toast.success("Тренажер успешно удален!");
                        fetch();
                    } else {
                        toast.error(response.error?.detail ?? "Ошибка удаления");
                    }
                } catch (e) {
                    console.error(e);
                    toast.error("Ошибка при удалении!");
                }
            }
        });
    }

    const onSubmit = (data: IAddOrUpdateGymEquipmentRequest) => {
        if (editingId) {
            update(editingId, data);
        } else {
            create(data);
        }
    }

    const handleEdit = (equipment: IGymEquipmentResponse) => {
        setEditingId(equipment.id);
        reset({
            equipmentId: equipment.equipment.id,
            gymId: equipment.gym.id,
            isActive: equipment.isActive,
            openedAt: equipment.openedAt,
            condition: equipment.condition,
        });
        setShowForm(true);

        setTimeout(() => {
            formRef.current?.scrollIntoView({ 
                behavior: 'smooth', 
                block: 'start' 
            });
        }, 100);
    }

    const handleCancel = () => {
        reset({
            equipmentId: "",
            gymId: currentGym?.id ?? "",
            isActive: true,
            openedAt: null,
            condition: EquipmentCondition.Operational
        });
        setEditingId(null);
        setShowForm(false);
        clearErrors();
    }

    const getConditionLabel = (condition: EquipmentCondition): string => {
        return equipmentConditionsOptions.find(opt => opt.value === condition)?.label ?? condition;
    }

    const getConditionTag = (condition: EquipmentCondition) => {
        const config = {
            [EquipmentCondition.Operational]: {
                color: 'success',
                icon: <CheckCircleOutlined />
            },
            [EquipmentCondition.Maintenance]: {
                color: 'warning',
                icon: <ToolOutlined />
            },
            [EquipmentCondition.UnderRepair]: {
                color: 'error',
                icon: <WarningOutlined />
            }
        };

        const conf = config[condition];
        return (
            <Tag color={conf.color} icon={conf.icon}>
                {getConditionLabel(condition)}
            </Tag>
        );
    }

    const columns = [
        {
            title: <><ToolOutlined className="mr-2" />Тренажер</>,
            key: 'equipment',
            render: (record: IGymEquipmentResponse) => (
                <div>
                    <div className="font-semibold text-gray-900">
                        {record.equipment.name}
                    </div>
                    <div className="text-xs text-gray-500">
                        {record.equipment.brand.name}
                    </div>
                </div>
            )
        },
        {
            title: <><HomeOutlined className="mr-2" />Зал</>,
            key: 'gym',
            render: (record: IGymEquipmentResponse) => record.gym.name
        },
        {
            title: 'Состояние',
            key: 'condition',
            render: (record: IGymEquipmentResponse) => getConditionTag(record.condition)
        },
        {
            title: 'Статус',
            key: 'isActive',
            render: (record: IGymEquipmentResponse) => (
                record.isActive ? (
                    <Tag color="success" icon={<CheckCircleOutlined />}>Активен</Tag>
                ) : (
                    <Tag color="default" icon={<CloseCircleOutlined />}>Неактивен</Tag>
                )
            )
        },
        {
            title: <><CalendarOutlined className="mr-2" />Дата открытия</>,
            key: 'openedAt',
            render: (record: IGymEquipmentResponse) => 
                record.openedAt 
                    ? dayjs(record.openedAt).format('DD.MM.YYYY HH:mm')
                    : <span className="text-gray-400">—</span>
        },
        {
            title: 'Действия',
            key: 'actions',
            render: (record: IGymEquipmentResponse) => (
                <Space>
                    <Button 
                        type="link" 
                        icon={<EditOutlined />}
                        onClick={() => handleEdit(record)}
                    >
                        Изменить
                    </Button>
                    <Button 
                        type="link" 
                        danger 
                        icon={<DeleteOutlined />}
                        onClick={() => deleteEquipment(record.id)}
                    >
                        Удалить
                    </Button>
                </Space>
            )
        }
    ];

    return (
        <div className="container mx-auto p-6">
            {/* Header */}
            <Card className="mb-6 shadow-md">
                <div className="flex justify-between items-center">
                    <div>
                        <h1 className="text-3xl font-bold text-gray-800 flex items-center gap-2">
                            <ToolOutlined className="text-blue-500" />
                            Тренажеры
                        </h1>
                        <p className="text-gray-500 mt-1">
                            Управление оборудованием зала
                        </p>
                    </div>
                    {!showForm && (
                        <Button 
                            type="primary" 
                            size="large"
                            icon={<PlusOutlined />}
                            onClick={() => setShowForm(true)}
                            className="shadow-lg"
                        >
                            Добавить тренажер
                        </Button>
                    )}
                </div>
            </Card>

            {/* Форма */}
            {showForm && (
                <Card 
                    ref={formRef}
                    title={
                        <span className="text-lg">
                            {editingId ? <><EditOutlined className="mr-2" />Редактировать</> : <><PlusOutlined className="mr-2" />Новый тренажер</>}
                        </span>
                    }
                    className="mb-6 shadow-md"
                    extra={
                        <Button onClick={handleCancel}>
                            Отмена
                        </Button>
                    }
                >
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            {/* Equipment */}
                            <div>
                                <label className="text-sm font-medium mb-2 flex items-center gap-2">
                                    <ToolOutlined />
                                    Тренажер *
                                </label>
                                <Controller
                                    name="equipmentId"
                                    control={control}
                                    render={({ field, fieldState: { error } }) => (
                                        <div>
                                            <UniversalSelect
                                                mode="infinite"
                                                pageSize={20}
                                                fetchPage={async (page, pageSize) => {
                                                    const params = new URLSearchParams({
                                                        PageNumber: page.toString(),
                                                        PageSize: pageSize.toString(),
                                                    });
                                    
                                                    const query = `/v1/equipments?${params}`;
                                                    const response = await apiService.get<ListResponse<IEquipmentResponse>>(query);
                                                    
                                                    if (response.success && response.data) {
                                                        const items = response.data?.items ?? [];
                                                        
                                                        return {
                                                            items: items.map((u: IEquipmentResponse) => ({
                                                                value: u.id,
                                                                label: u.name,
                                                            })),
                                                            hasMore: (response.data.totalPages ?? 0) > page
                                                        };
                                                    }
                                                    
                                                    return {
                                                        items: [],
                                                        hasMore: false
                                                    };
                                                }}
                                                placeholder="Выберите тренажер"
                                                allowClear
                                                value={(field.value as string) ?? undefined}
                                                onChange={field.onChange}
                                                style={{ width: '100%' }}
                                            />
                                            {error && <div className="text-red-500 text-xs mt-1">{error.message}</div>}
                                        </div>
                                    )}
                                />
                            </div>

                            {/* Gym */}
                            <div>
                                <label className="text-sm font-medium mb-2 flex items-center gap-2">
                                    <HomeOutlined />
                                    Зал *
                                </label>
                                <Controller
                                    name="gymId"
                                    control={control}
                                    render={({ field, fieldState: { error } }) => {
                                        const options: OptionType[] = [
                                            { 
                                                value: currentGym?.id ?? "", 
                                                label: currentGym?.name ?? "",
                                            }
                                        ];

                                        return (
                                            <div>
                                                <UniversalSelect
                                                    mode="static"
                                                    options={options}
                                                    placeholder="Выберите зал"
                                                    allowClear
                                                    value={(field.value as string) ?? undefined}
                                                    onChange={field.onChange}
                                                    style={{ width: '100%' }}
                                                />
                                                {error && <div className="text-red-500 text-xs mt-1">{error.message}</div>}
                                            </div>
                                        )
                                    }}
                                />
                            </div>

                            {/* Condition */}
                            <div>
                                <label className="block text-sm font-medium mb-2">
                                    Состояние *
                                </label>
                                <Controller
                                    name="condition"
                                    control={control}
                                    render={({ field, fieldState }) => (
                                        <div>
                                            <Select
                                                {...field}
                                                className="w-full"
                                                status={fieldState.error ? 'error' : ''}
                                                size="large"
                                                options={equipmentConditionsOptions.map(opt => ({
                                                    value: opt.value,
                                                    label: opt.label
                                                }))}
                                            />
                                            {fieldState.error && (
                                                <div className="text-red-500 text-xs mt-1">
                                                    {fieldState.error.message}
                                                </div>
                                            )}
                                        </div>
                                    )}
                                />
                            </div>

                            {/* IsActive */}
                            <div className="flex items-center">
                                <Controller
                                    name="isActive"
                                    control={control}
                                    render={({ field, fieldState }) => (
                                        <div>
                                            <Checkbox
                                                checked={field.value}
                                                onChange={(e) => {
                                                    field.onChange(e.target.checked);
                                                    if (e.target.checked) {
                                                        setValue("openedAt", null);
                                                    }
                                                }}
                                            >
                                                <span className="font-medium">Активен</span>
                                            </Checkbox>
                                            {fieldState.error && (
                                                <div className="text-red-500 text-xs mt-1">
                                                    {fieldState.error.message}
                                                </div>
                                            )}
                                        </div>
                                    )}
                                />
                            </div>

                            {(!isActive || condition === EquipmentCondition.Maintenance || condition === EquipmentCondition.UnderRepair) && (
                                <div className="md:col-span-2">
                                    <label className="text-sm font-medium mb-2 flex items-center gap-2">
                                        <CalendarOutlined />
                                        Дата открытия *
                                    </label>
                                    <Controller
                                        name="openedAt"
                                        control={control}
                                        render={({ field, fieldState }) => (
                                            <div>
                                                <DatePicker
                                                    showTime
                                                    format="DD.MM.YYYY HH:mm"
                                                    value={field.value ? dayjs(field.value) : null}
                                                    onChange={(date) => field.onChange(date ? date.toDate() : null)}
                                                    className="w-full"
                                                    size="large"
                                                    status={fieldState.error ? 'error' : ''}
                                                    placeholder="Выберите дату"
                                                />
                                                {fieldState.error && (
                                                    <div className="text-red-500 text-xs mt-1">
                                                        {fieldState.error.message}
                                                    </div>
                                                )}
                                            </div>
                                        )}
                                    />
                                </div>
                            )}
                        </div>

                        <div className="flex gap-3 mt-6">
                            <Button 
                                type="primary" 
                                htmlType="submit"
                                loading={submitting}
                                size="large"
                                icon={editingId ? <EditOutlined /> : <PlusOutlined />}
                            >
                                {editingId ? "Обновить" : "Создать"}
                            </Button>
                            <Button 
                                onClick={handleCancel}
                                size="large"
                            >
                                Отмена
                            </Button>
                        </div>
                    </form>
                </Card>
            )}

            {/* Table */}
            <Card className="shadow-md">
                <Table
                    columns={columns}
                    dataSource={gymEquipments}
                    rowKey="id"
                    loading={loading}
                    pagination={false}
                    locale={{
                        emptyText: <Empty description="Нет данных" />
                    }}
                />

                {/* Pagination */}
                <div className="flex justify-center mt-6">
                    <Pagination
                        current={page}
                        pageSize={pageSize}
                        total={total}
                        onChange={(newPage, newPageSize) => {
                            setPage(newPage);
                            setPageSize(newPageSize);
                        }}
                        showSizeChanger
                        showTotal={(total) => `Всего ${total} записей`}
                        pageSizeOptions={['10', '20', '50', '100']}
                    />
                </div>
            </Card>
        </div>
    );
}
