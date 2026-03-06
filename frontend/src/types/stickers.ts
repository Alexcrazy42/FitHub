export interface IStickerGroupResponse {
  id: string;
  name: string;
  isActive: boolean;
}

export interface IStickerResponse {
  id: string;
  name: string;
  groupId: string;
  fileId: string;
  position: number;
}
