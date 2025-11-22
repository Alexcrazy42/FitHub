import React, { useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import { Modal, Button, Form, Input, Select, DatePicker, TimePicker, Popconfirm, Checkbox } from "antd";

type TrainingEvent = {
  id: string;
  title: string;
  trainer: string;
  date: string; // YYYY-MM-DD
  start: string; // HH:mm
  end: string;   // HH:mm
  color: string;
};

const sampleTrainers = [
  { value: "Иван", color: "#60A5FA" },
  { value: "Мария", color: "#34D399" },
  { value: "Ольга", color: "#F59E0B" },
];

export const ScheduleOld: React.FC = () => {
  const [events, setEvents] = useState<TrainingEvent[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingEvent, setEditingEvent] = useState<TrainingEvent | null>(null);
  const [form] = Form.useForm();
  const [selectedTrainer, setSelectedTrainer] = useState<string | null>(null);
  const [onlyShowSelected, setOnlyShowSelected] = useState(false);

  const openModal = (event?: TrainingEvent) => {
    if (event) {
      setEditingEvent(event);
      form.setFieldsValue({
        title: event.title,
        trainer: event.trainer,
        date: dayjs(event.date),
        timeStart: dayjs(`${event.date}T${event.start}`),
        timeEnd: dayjs(`${event.date}T${event.end}`),
      });
    } else {
      setEditingEvent(null);
      form.resetFields();
    }
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingEvent(null);
    form.resetFields();
  };

  const saveEvent = (values: any) => {
    const date: Dayjs = values.date;
    const startTime: Dayjs = values.timeStart;
    const endTime: Dayjs = values.timeEnd;
    const newEvent: TrainingEvent = {
      id: editingEvent ? editingEvent.id : String(Date.now()),
      title: values.title,
      trainer: values.trainer,
      date: date.format("YYYY-MM-DD"),
      start: startTime.format("HH:mm"),
      end: endTime.format("HH:mm"),
      color: sampleTrainers.find(t => t.value === values.trainer)?.color || "#60A5FA",
    };

    if (editingEvent) {
      setEvents(prev => prev.map(ev => (ev.id === editingEvent.id ? newEvent : ev)));
    } else {
      setEvents(prev => [...prev, newEvent]);
    }
    closeModal();
  };

  const deleteEvent = (id: string) => {
    setEvents(prev => prev.filter(ev => ev.id !== id));
  };

  const filteredEvents = events.filter(ev =>
    selectedTrainer ? (onlyShowSelected ? ev.trainer === selectedTrainer : true) : true
  );

  // Простая таблица 7 дней недели
  const weekDays = Array.from({ length: 7 }, (_, i) => dayjs().startOf("week").add(i, "day"));

  return (
    <div style={{ padding: 20 }}>
      <h1>Расписание тренировок</h1>
      <div style={{ marginBottom: 16 }}>
        <Select
          placeholder="Выделить тренера"
          allowClear
          style={{ width: 180, marginRight: 8 }}
          value={selectedTrainer ?? undefined}
          onChange={v => setSelectedTrainer(v ?? null)}
          options={sampleTrainers.map(t => ({ label: t.value, value: t.value }))}
        />
        <Checkbox checked={onlyShowSelected} onChange={e => setOnlyShowSelected(e.target.checked)} style={{ marginRight: 8 }}>
          Показать только выбранного
        </Checkbox>
        <Button type="primary" onClick={() => openModal()}>
          Создать
        </Button>
      </div>

      <table style={{ width: "100%", borderCollapse: "collapse" }}>
        <thead>
          <tr>
            {weekDays.map(day => (
              <th key={day.format("YYYY-MM-DD")} style={{ border: "1px solid #ccc", padding: 8 }}>
                {day.format("ddd DD.MM")}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          <tr>
            {weekDays.map(day => {
              const dayEvents = filteredEvents.filter(ev => ev.date === day.format("YYYY-MM-DD"));
              return (
                <td key={day.format("YYYY-MM-DD")} style={{ border: "1px solid #ccc", verticalAlign: "top", padding: 4, minHeight: 100 }}>
                  {dayEvents.map(ev => (
                    <div
                      key={ev.id}
                      style={{
                        backgroundColor: ev.color,
                        color: "#fff",
                        padding: "2px 4px",
                        marginBottom: 4,
                        borderRadius: 4,
                        cursor: "pointer",
                      }}
                      onClick={() => openModal(ev)}
                      draggable
                      onDragStart={e => e.dataTransfer.setData("text/plain", ev.id)}
                    >
                      {ev.title} ({ev.trainer}) {ev.start}-{ev.end}
                    </div>
                  ))}
                </td>
              );
            })}
          </tr>
        </tbody>
      </table>

      <Modal title={editingEvent ? "Редактировать тренировку" : "Создать тренировку"} open={isModalOpen} onCancel={closeModal} footer={null} destroyOnClose>
        <Form form={form} layout="vertical" onFinish={saveEvent}>
          <Form.Item name="title" label="Название" rules={[{ required: true }]}>
            <Input />
          </Form.Item>
          <Form.Item name="trainer" label="Тренер" rules={[{ required: true }]}>
            <Select options={sampleTrainers.map(t => ({ label: t.value, value: t.value }))} />
          </Form.Item>
          <Form.Item name="date" label="Дата" rules={[{ required: true }]}>
            <DatePicker className="w-full" />
          </Form.Item>
          <div style={{ display: "flex", gap: 8 }}>
            <Form.Item name="timeStart" label="Время начала" rules={[{ required: true }]}>
              <TimePicker format="HH:mm" />
            </Form.Item>
            <Form.Item name="timeEnd" label="Время окончания" rules={[{ required: true }]}>
              <TimePicker format="HH:mm" />
            </Form.Item>
          </div>
          <div style={{ display: "flex", justifyContent: "space-between", marginTop: 16 }}>
            <div>
              <Button type="primary" htmlType="submit">{editingEvent ? "Сохранить" : "Создать"}</Button>
              {editingEvent && (
                <Popconfirm title="Удалить тренировку?" onConfirm={() => deleteEvent(editingEvent.id)}>
                  <Button danger style={{ marginLeft: 8 }}>Удалить</Button>
                </Popconfirm>
              )}
            </div>
            <Button onClick={closeModal}>Отмена</Button>
          </div>
        </Form>
      </Modal>
    </div>
  );
};

export default ScheduleOld;
