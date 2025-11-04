using FitHub.Application.Files;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Files;
using FitHub.Domain.Files;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Files;

public static class FileExtensions
{
    public static GetPresignedUrlCommand ToPresignedUrlCommand(this GetPresignedUrlRequest? request)
    {
        if (request?.File is null)
        {
            throw new ValidationException("Файл не может быть пустым!");
        }

        return new GetPresignedUrlCommand
        {
            File = request.File.Required()
        };
    }

    public static PresignedUrlResponse ToPresignedUrlResponse(this PresignedUrlResult result)
    {
        return new PresignedUrlResponse()
        {
            Url = result.Url,
            FileId = result.FileId,
            ObjectKey = result.ObjectKey
        };
    }

    public static FileResponse ToFileResponse(this FileEntity fileEntity)
    {
        return new FileResponse()
        {
            Id = fileEntity.Id.ToString(),
            FileName = fileEntity.FileName
        };
    }
}
