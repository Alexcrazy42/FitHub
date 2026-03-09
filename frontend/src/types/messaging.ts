import { UserResponse } from "./auth";

export enum ChatType {
  OneToOne = "OneToOne",
  Group = "Group",
}

export enum TagUserAttachmentType {
    Online = "Online",
    All = "All",
    ConcreteUser = "ConcreteUser"
}

export enum MessageAttachmentType {
    CreateGroup = "CreateGroup",
    Photo = "Photo",
    Link = "Link",
    TagUser = "TagUser",
    InviteUser = "InviteUser",
    ExcludeUser = "ExcludeUser",
    Sticker = "Sticker",
    Document = "Document",
    Voice = "Voice"
}


export interface IInitiatorAndTargetUserRequest {
    chatId: string;
    targetUserId: string;
}

export interface ICreateChatRequest {
    type: ChatType;
    participantUserIds: string[];
}

export interface IChatParticipantResponse {
    id: string;
    chatId: string;
    user: UserResponse;
    joinedAt: string;
}

export interface IChatResponse {
    id: string;
    type: ChatType;
    name: string | null;
    participants: IChatParticipantResponse[]
}

export interface ICreateLinkAttachmentRequest {
    url: string;
    title: string;
    caption: string;
    photoUrl: string;
}

export interface ICreateTagUserAttachmentRequest {
    name: string;
    type: TagUserAttachmentType;
    taggedUserId: string;
}

export interface ICreatePhotoAttachmentRequest {
    fileId: string
}

export interface ICreateStickerAttachmentRequest {
    stickerId: string;
    fileId: string;
    name: string;
}

export interface ICreateDocumentAttachmentRequest {
    fileId: string;
    fileName: string;
    fileSize: number;
    mimeType: string;
}

export interface ICreateVoiceAttachmentRequest {
    fileId: string;
    durationMs: number;
    mimeType: string;
    peaks: number[];
}

export interface ICreateMessageRequest {
    chatId: string;
    messageText: string;
    replyMessageId: string | null;
    links: ICreateLinkAttachmentRequest[];
    tags: ICreateTagUserAttachmentRequest[];
    photos: ICreatePhotoAttachmentRequest[];
    stickers: ICreateStickerAttachmentRequest[];
    documents?: ICreateDocumentAttachmentRequest[];
    voices?: ICreateVoiceAttachmentRequest[];
}

export interface IMessageAttachmentResponse {
    id: string;
    messageId: string;
    type: MessageAttachmentType;
    data: string;
    createdAt: string;
    createdBy: UserResponse;
    updatedAt: string;
    updatedBy: UserResponse
}

export interface IMessageResponse {
    id: string;
    chatId: string;
    messageText: string;
    replyMessage: IMessageResponse | null;
    forwardedMessage: IMessageResponse | null;
    attachments: IMessageAttachmentResponse[];
    createdAt: string;
    createdBy: UserResponse;
    updatedAt: string;
    updatedBy: UserResponse
}

export interface IMessageReadRequest {
    maxMessageId: string;
}

export interface IUpdateMessageRequest {
    messageText: string;
    replyMessageId: string | null;
    links: ICreateLinkAttachmentRequest[];
    tags: ICreateTagUserAttachmentRequest[];
    photos: ICreatePhotoAttachmentRequest[];
}


export interface IChatMessageResponse {
    id: string;
    chat: IChatResponse;
    lastMessage: IMessageResponse;
    unreadCount: number;
    lastMessageTime: string;
}

export interface IGetMessagesRequest {
    chatId: string;
    isDescending: boolean;
    fromUnread: boolean;
    loadLastMessages: boolean;
    from: Date | null
}


export enum ConnectionState {
  DISCONNECTED = 'disconnected',
  CONNECTING = 'connecting',
  CONNECTED = 'connected',
  RECONNECTING = 'reconnecting',
  FAILED = 'failed',
}