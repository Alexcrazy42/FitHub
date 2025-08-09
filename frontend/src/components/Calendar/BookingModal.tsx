import React from "react";
import { Modal, Form, Input, TimePicker, Button, message } from "antd";
import type { RangePickerProps } from "antd/es/date-picker";
import dayjs from "dayjs";
import { CreateBookingDto } from "../../types/gymTypes";

interface BookingModalProps {
  open: boolean;
  onCancel: () => void;
  zoneId: number;
  onCreate: (booking: CreateBookingDto) => void;
}

const BookingModal: React.FC<BookingModalProps> = ({ 
  open, 
  onCancel, 
  zoneId,
  onCreate 
}) => {
  const [form] = Form.useForm();

  const onFinish = async (values: { 
  time: RangePickerProps["value"]; 
  description: string 
}) => {
  if (!values.time || !values.time[0] || !values.time[1]) {
    message.error("Укажите время начала и окончания");
    return;
  }

  const booking: CreateBookingDto = {
    zoneId,
    startTime: values.time[0].toISOString(),
    endTime: values.time[1].toISOString(),
    description: values.description,
  };

  try {
    await onCreate(booking);
    form.resetFields();
    onCancel();
  } catch (error) {
    message.error("Не удалось создать тренировку");
  }
};

  return (
    <Modal
      title="Создать тренировку"
      open={open}
      onCancel={onCancel}
      footer={null}
    >
      <Form form={form} onFinish={onFinish}>
        <Form.Item
          name="time"
          label="Время"
          rules={[{ required: true, message: "Укажите время" }]}
        >
          <TimePicker.RangePicker 
            format="HH:mm" 
            minuteStep={15}
          />
        </Form.Item>

        <Form.Item name="description" label="Описание">
          <Input.TextArea />
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit">
            Сохранить
          </Button>
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default BookingModal;