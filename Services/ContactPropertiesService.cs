using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reloop.Exceptions;
using Reloop.Validation;
using static Reloop.Models.ContactModels;

namespace Reloop.Services;

/** Manages contact property definitions. */
public class ContactPropertiesService
{
    private const string PropertiesBase = "/api/contacts/v1/properties";
    private static readonly HttpMethod PatchMethod = new("PATCH");

    private readonly ReloopClient _client;

    internal ContactPropertiesService(ReloopClient client)
    {
        _client = client;
    }

    public Task<ContactPropertyResponse?> CreateAsync(CreatePropertyParams parameters)
    {
        var body = ValidateCreateParams(parameters);
        return _client.FetchAsync<ContactPropertyResponse>(HttpMethod.Post, PropertiesBase + "/create", body);
    }

    public Task<PropertyListResponse?> ListAsync(ListPropertiesParams? parameters = null)
    {
        var query = ValidateListParams(parameters);
        return _client.FetchAsync<PropertyListResponse>(HttpMethod.Get, PropertiesBase + "/list", null, query);
    }

    public Task<ContactPropertyResponse?> UpdateAsync(string id, UpdatePropertyParams? parameters = null)
    {
        var propertyId = RequirePropertyId(id, "id");
        var body = ValidateUpdateParams(parameters);
        return _client.FetchAsync<ContactPropertyResponse>(PatchMethod, PropertiesBase + "/" + propertyId, body);
    }

    public Task<DeletePropertyResponse?> DeleteAsync(string id)
    {
        var propertyId = RequirePropertyId(id, "id");
        return _client.FetchAsync<DeletePropertyResponse>(HttpMethod.Delete, PropertiesBase + "/" + propertyId);
    }

    private static string RequirePropertyId(string? id, string field)
    {
        try
        {
            return Validators.RequireNonEmptyString(id, field);
        }
        catch (ReloopValidationException)
        {
            throw new ReloopValidationException(
                "Property " + field + " is required and must be a non-empty string.", field);
        }
    }

    private static string RequirePropertyName(string? name, string field)
    {
        if (name == null)
        {
            throw new ReloopValidationException(
                "Property " + field + " is required and must be a string.", field);
        }

        var trimmed = name.Trim();
        if (trimmed.Length < 1)
        {
            throw new ReloopValidationException(
                "Property " + field + " must be at least 1 character.", field);
        }

        if (trimmed.Length > 255)
        {
            throw new ReloopValidationException(
                "Property " + field + " must be at most 255 characters.", field);
        }

        return trimmed;
    }

    private static string RequirePropertyType(string? type, string field)
    {
        if (type == null || (type != PropertyType.String && type != PropertyType.Number))
        {
            throw new ReloopValidationException(
                "Property " + field + " must be \"string\" or \"number\".", field);
        }

        return type;
    }

    private static Dictionary<string, object?> ValidateCreateParams(CreatePropertyParams? parameters)
    {
        if (parameters == null)
        {
            throw new ReloopValidationException("create params are required and must be an object.", "params");
        }

        var body = new Dictionary<string, object?>
        {
            ["name"] = RequirePropertyName(parameters.Name, "name"),
            ["type"] = RequirePropertyType(parameters.Type, "type"),
        };

        if (parameters.FallbackValue != null)
        {
            body["fallbackValue"] = parameters.FallbackValue;
        }

        return body;
    }

    private static Dictionary<string, object?> ValidateUpdateParams(UpdatePropertyParams? parameters)
    {
        return new Dictionary<string, object?>
        {
            ["fallbackValue"] = parameters?.FallbackValue,
        };
    }

    private static Dictionary<string, string?> ValidateListParams(ListPropertiesParams? parameters)
    {
        var query = new Dictionary<string, string?>();
        if (parameters == null)
        {
            return query;
        }

        if (parameters.Page.HasValue)
        {
            Validators.RequirePage(parameters.Page.Value, "page");
            query["page"] = parameters.Page.Value.ToString();
        }

        if (parameters.Limit.HasValue)
        {
            Validators.RequireLimit(parameters.Limit.Value, 1, 100, "limit");
            query["limit"] = parameters.Limit.Value.ToString();
        }

        if (parameters.Search != null)
        {
            query["search"] = parameters.Search;
        }

        if (parameters.Type != null)
        {
            RequirePropertyType(parameters.Type, "type");
            query["type"] = parameters.Type;
        }

        return query;
    }
}
