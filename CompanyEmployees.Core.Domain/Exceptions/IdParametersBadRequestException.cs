namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class IdParametersBadRequestException : BadRequestException
{
    public IdParametersBadRequestException()
        : base("Parameter ids is null")
    {
    }
}
