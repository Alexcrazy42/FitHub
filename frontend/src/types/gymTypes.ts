export interface GymZone {
  id: number;
  name: string;
  maxCapacity: number;
  isAvailable: boolean;
}

export interface ZoneBooking {
  id: number;
  zoneId: number;
  startTime: string;
  endTime: string;
  status: "active" | "canceled";
  description?: string;
}

export interface CreateBookingDto {
  zoneId: number;
  startTime: string;
  endTime: string;
  description?: string;
}