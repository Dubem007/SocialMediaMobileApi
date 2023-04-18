using Application.Helpers;
using Shared.DataTransferObjects;

namespace Application.Contracts;

public interface IAuthenticationService : IAutoDependencyService
{
    Task<SuccessResponse<UserLoginResponse>> Login(UserLoginDTO model);
    Task<SuccessResponse<RefreshTokenResponse>> GetRefreshToken(RefreshTokenDTO model);
    Task<SuccessResponse<GetSetPasswordDto>> SetPassword(SetPasswordDTO model);
    Task<SuccessResponse<object>> ResetPassword(ResetPasswordDTO model);
    Task<SuccessResponse<GetConifrmedTokenUserDto>> ConfirmToken(VerifyTokenDTO model);
    Task<SuccessResponse<object>> SendToken(SendTokenInputDto model);
    Task<SuccessResponse<ReferenceTokenResponseDto>> VerifyOtp(VerifyTokenDTO model);
    Task<SuccessResponse<GetInitialMemberTokenResponseDto>> GetInitialMemberByReferenceToken(ReferenceTokenInputDto model);
    Task<SuccessResponse<object>> ChangePassword(ChangePasswordDto model);

}