import { IMessageResponse, MessageAttachmentType } from "../messaging";

const systemAttachmentTypes = [
    MessageAttachmentType.CreateGroup,
    MessageAttachmentType.InviteUser,
    MessageAttachmentType.ExcludeUser
  ];

export const isSystemMessage = (message: IMessageResponse) : boolean => {
    return message.attachments.some(
        attachment => systemAttachmentTypes.includes(attachment.type)
    )
}