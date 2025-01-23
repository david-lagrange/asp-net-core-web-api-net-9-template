namespace CompanyEmployees.Core.Domain.Exceptions;

public sealed class RoleNotFoundException : NotFoundException
{
    public RoleNotFoundException(string role)
        : base($"The role '{role}' doesn't exist in the database.")
    {
    }
}