using Common.Model.Common;
using Common.Model.UsGroup;
using Common.Model.UsUser;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebBlazor.Services;

public class UserApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserApiClient> _logger;

    public UserApiClient(HttpClient httpClient, ILogger<UserApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    #region User Methods

    public async Task<List<UsUserViewModel>> GetUserAllSync()
    {
        try
        {
            _logger.LogInformation("Fetching users from API...");
            var response = await _httpClient.GetFromJsonAsync<List<UsUserViewModel>>("api/User");
            _logger.LogInformation("Users fetched successfully.");
            return response ?? new();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error fetching users.");
            return new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error fetching users.");
            return new();
        }
    }

    public async Task<UsUserViewModel?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/User/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return JsonSerializer.Deserialize<UsUserViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user {id}");
            return null;
        }
    }
    public async Task<PaginatedResponse<UsUserViewModel>> GetPaginatedUsersAsync(PaginatedRequest request)
    {
        try
        {
            _logger.LogInformation("Fetching paginated users from API...");

            var queryString = $"?PageNumber={request.PageNumber}" +
                             $"&PageSize={request.PageSize}";

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                queryString += $"&SearchTerm={Uri.EscapeDataString(request.SearchTerm)}";
            }

            if (!string.IsNullOrWhiteSpace(request.SortColumn))
            {
                queryString += $"&SortColumn={request.SortColumn}" +
                              $"&SortDirection={request.SortDirection}";
            }

            var response = await _httpClient.GetFromJsonAsync<PaginatedResponse<UsUserViewModel>>(
                $"api/User/paginated{queryString}"
            );

            _logger.LogInformation("Paginated users fetched successfully.");
            return response ?? new PaginatedResponse<UsUserViewModel>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching paginated users");
            return new PaginatedResponse<UsUserViewModel>();
        }
    }
    public async Task<UsUserViewModel?> CreateUserAsync(UsUserCreateModel request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/User", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return JsonSerializer.Deserialize<UsUserViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }

            //  Lỗi từ API
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Create user failed: {response.StatusCode} - {error}");

            // Parse error message nếu có
            var errorMessage = ExtractErrorMessage(error);

            // Ném exception với message rõ ràng
            throw new HttpRequestException(errorMessage ?? "Tạo người dùng thất bại");
        }
        catch (HttpRequestException)
        {
            // Re-throw để UserList.razor bắt được
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            throw new HttpRequestException($"Lỗi kết nối: {ex.Message}");
        }
    }

    public async Task<UsUserViewModel?> UpdateUserAsync(UsUserUpdateModel request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/User/{request.Id}", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    return JsonSerializer.Deserialize<UsUserViewModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                // Nếu không có content, trả về object với data từ request
                return new UsUserViewModel
                {
                    Id = request.Id,
                    Name = request.Name ?? string.Empty,
                    GroupId = request.GroupId ?? string.Empty,
                    Email = request.Email,
                    Phone = request.Phone,
                    Gender = request.Gender,
                    CMND = request.CMND,
                    Address = request.Address,
                    Note = request.Note,
                    RowStatus = request.RowStatus
                };
            }

            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Update user failed: {response.StatusCode} - {error}");
            
            var errorMessage = ExtractErrorMessage(error);
            throw new HttpRequestException(errorMessage ?? $"Cập nhật người dùng thất bại: {error}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            throw new HttpRequestException($"Lỗi kết nối: {ex.Message}");
        }
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/User/{userId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user {userId}");
            throw;
        }
    }

    private string? ExtractErrorMessage(string errorContent)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(errorContent)) return null;

            using var doc = JsonDocument.Parse(errorContent);
            if (doc.RootElement.TryGetProperty("message", out var message))
            {
                return message.GetString();
            }
            if (doc.RootElement.TryGetProperty("Message", out var msg))
            {
                return msg.GetString();
            }
        }
        catch
        {
            // Ignore parse errors
        }
        return null;
    }

    #endregion

    #region Group Methods

    public async Task<List<UsGroupViewModel>> GetAllGroupsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching groups from API...");
            var response = await _httpClient.GetFromJsonAsync<List<UsGroupViewModel>>("api/Group");
            _logger.LogInformation("Groups fetched successfully.");
            return response ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching groups");
            return new();
        }
    }

    #endregion
}