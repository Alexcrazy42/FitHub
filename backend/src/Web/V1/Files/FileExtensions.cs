using FitHub.Application.Files;
using FitHub.Common.Utilities.System;
using FitHub.Contracts.V1.Files;
using FluentValidation;

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
}
