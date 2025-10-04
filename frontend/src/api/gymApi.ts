import { GymZone, ZoneBooking, CreateBookingDto } from "../types/gymTypes";

export const fetchGymZones = async (): Promise<GymZone[]> => {
  return [
    { id: 1, name: "Тренажерный зал", maxCapacity: 20, isAvailable: true },
    { id: 2, name: "Зал йоги", maxCapacity: 10, isAvailable: true },
    { id: 3, name: "Бассейн", maxCapacity: 8, isAvailable: false },
  ];
};

export const fetchZoneBookings = async (zoneId: number): Promise<ZoneBooking[]> => {
  const mockData: Record<number, ZoneBooking[]> = {
    1: [
      {
        id: 101,
        zoneId: 1,
        startTime: "2024-06-10T10:00:00",
        endTime: "2024-06-10T11:30:00",
        status: "active",
        description: "Силовая тренировка",
      },
    ],
    2: [],
    3: [],
  };
  return mockData[zoneId] || [];
};

export const createBooking = async (booking: CreateBookingDto): Promise<{ success: boolean }> => {
  return { success: true };
};