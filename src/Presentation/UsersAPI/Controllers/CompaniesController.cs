using ApiTemplate.Application.Features.Companies.Dtos;
using ApiTemplate.Application.Features.Companies.Onboarding;
using ApiTemplate.Application.Features.Companies.Onboarding.Dtos;
using ApiTemplate.Application.Features.Companies.Profile;
using ApiTemplate.Application.Features.Companies.Profile.Dtos;
using ApiTemplate.Application.Features.Companies.Registration;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.UsersAPI.Controllers;

public class CompaniesController : BaseApiController
{
    [HttpPost("{id:guid}/onboarding/apply-coupon")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.CompanyInvalidStatus,
        ErrorCodes.CouponNotFound,
        ErrorCodes.CompanyHasCoupon,
        ErrorCodes.CouponExpired,
        ErrorCodes.CouponAlreadyApplied
    )]
    public Task<CompanyCouponDto> ApplyCouponAsync(
        Guid id,
        ApplyCoupon.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<ApplyCoupon.Request>();
        req.CompanyId = id;


        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/basic-information")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyBasicInfoDto> GetBasicInfoAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyBasicInfo.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/onboarding/basic-information")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus)]
    public Task SaveBasicInfoAsync(
        Guid id,
        SaveCompanyBasicInfo.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyBasicInfo.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/legal-notices")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyLegalNoticesDto> GetLegalNoticesAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyLegalNotices.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/onboarding/legal-notices")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus)]
    public Task SetLegalNoticesAsync(
        Guid id,
        SaveCompanyLegalNotices.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyLegalNotices.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/payment-type")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyPaymentTypeDto> GetPaymentTypeAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyPaymentType.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/onboarding/payment-type")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus)]
    public Task SavePaymentTypeAsync(
        Guid id,
        SaveCompanyPaymentType.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyPaymentType.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/contacts")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<BaseCompanyContactsDto> GetContactsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyContacts.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/onboarding/contacts")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus)]
    public Task SaveContactsAsync(
        Guid id,
        SaveCompanyContacts.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyContacts.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/billing-taxes")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyBillingTaxesDto> GetBillingTaxesAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyBillingTaxes.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/onboarding/billing-taxes")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus)]
    public Task SaveBillingTaxesAsync(
        Guid id,
        SaveCompanyBillingTaxes.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyBillingTaxes.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/associates")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyAssociatesDto> GetAssociatesAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyAssociates.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/onboarding/associates")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus)]
    public Task<List<Guid>> SaveAssociatesAsync(
        Guid id,
        SaveCompanyAssociates.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyAssociates.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/documents")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<List<CompanyDocumentDto>> GetDocumentsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyDocuments.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/onboarding/documents/upload")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.CompanyInvalidStatus,
        ErrorCodes.CompanyDocumentNotFound,
        ErrorCodes.MaxCompanyDocumentFiles
    )]
    public Task<BaseCompanyDocumentFileDto> UploadDocumentAsync(
        Guid id,
        UploadCompanyFile.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<UploadCompanyFile.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpDelete("{id:guid}/onboarding/documents/{documentFileId:guid}")]
    [OperationErrors(
        ErrorCodes.CompanyDocumentFileNotFound,
        ErrorCodes.CompanyInvalidStatus,
        ErrorCodes.ParamsMissMatch
    )]
    public Task<Guid> DeleteDocumentAsync(
        Guid id,
        Guid documentFileId,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new DeleteCompanyFile.Request(id, documentFileId), cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/summary")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanySummaryDto> GetSummaryAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanySummary.Request(id), cancellationToken);
    }

    [HttpGet("{id:guid}/onboarding/summary/pdf")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<EncodedFileResponse> GetSummaryPdfAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanySummaryPdf.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/onboarding/finish")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus)]
    public Task FinishAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = new FinishOnboarding.Request()
        {
            CompanyId = id
        };

        return Mediator.Send(request, cancellationToken);
    }

    [HttpPost("{id:guid}/registration")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.InvalidSignToken,
        ErrorCodes.CompanyRegistrationAlreadySigned
    )]
    [AllowAnonymous]
    public Task<CompanySummaryDto> GetRegistrationAsync(
        Guid id,
        GetRegistration.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<GetRegistration.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPost("{id:guid}/registration/pdf")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.InvalidSignToken,
        ErrorCodes.CompanyRegistrationAlreadySigned
    )]
    [AllowAnonymous]
    public Task<EncodedFileResponse> GetRegistrationPdfAsync(
        Guid id,
        GetRegistrationPdf.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<GetRegistrationPdf.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPost("{id:guid}/registration/sign")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.InvalidSignToken,
        ErrorCodes.CompanyRegistrationAlreadySigned
    )]
    [AllowAnonymous]
    public Task SignRegistrationPdfAsync(
        Guid id,
        SignRegistrationPdf.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SignRegistrationPdf.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/profile")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileGeneralInfoDto> GetGeneralInformationAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileGeneralInfo.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/profile/logo")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<string?> UploadLogoAsync(
        Guid id,
        UploadCompanyLogoFile.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<UploadCompanyLogoFile.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/name")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task SaveNameAsync(
        Guid id,
        SaveCompanyName.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyName.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/general-information")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task SaveGeneralInformationAsync(
        Guid id,
        SaveCompanyGeneralInfo.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyGeneralInfo.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/categories")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task SaveCategoriesAsync(
        Guid id,
        SaveCompanyCategories.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyCategories.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/economic-sectors")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task SaveEconomicSectorsAsync(
        Guid id,
        SaveCompanySectors.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanySectors.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/about")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task SaveAboutAsync(
        Guid id,
        SaveCompanyAbout.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyAbout.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/profile/basic-information")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileBasicInfoDto> GetProfileBasicInfoAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileBasicInfo.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/basic-information")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInReview)]
    public Task SaveProfileBasicInformationAsync(
        Guid id,
        SaveCompanyProfileBasicInfo.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyProfileBasicInfo.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/profile/contacts")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileContactsDto> GetProfileContactsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileContacts.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/contacts")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task SaveProfileContactsAsync(
        Guid id,
        SaveCompanyProfileContacts.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyProfileContacts.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/profile/billing-taxes")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileBillingTaxesDto> GetProfileBillingTaxesAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileBillingTaxes.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/billing-taxes")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInReview)]
    public Task SaveProfileBillingTaxesAsync(
        Guid id,
        SaveCompanyProfileBillingTaxes.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyProfileBillingTaxes.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/electronic-invoice")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task SaveProfileElectronicInvoiceAsync(
        Guid id,
        SaveCompanyProfileElectronicInvoice.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyProfileElectronicInvoice.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/profile/associates")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileAssociatesDto> GetProfileAssociatesAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileAssociates.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/associates")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInReview)]
    public Task SaveProfileAssociatesAsync(
        Guid id,
        SaveCompanyProfileAssociates.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyProfileAssociates.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/profile/addresses")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileAddressesDto> GetProfileAddressesAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileAddresses.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/profile/addresses")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyAddressNotFound)]
    public Task<Guid> SaveProfileAddressAsync(
        Guid id,
        SaveCompanyAddress.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<SaveCompanyAddress.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpDelete("{id:guid}/profile/addresses/{addressId:guid}")]
    [OperationErrors(ErrorCodes.CompanyAddressNotFound, ErrorCodes.ParamsMissMatch)]
    public Task<Guid> DeleteProfileAddressAsync(
        Guid id,
        Guid addressId,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new DeleteCompanyAddress.Request(id, addressId), cancellationToken);
    }

    [HttpGet("{id:guid}/profile/documents")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileDocumentsDto> GetProfileDocumentsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileDocuments.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/profile/documents/upload")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.CompanyInReview,
        ErrorCodes.CompanyDocumentNotFound,
        ErrorCodes.InvalidCreditAvailability,
        ErrorCodes.MaxCompanyDocumentFiles
    )]
    public Task<CompanyProfileDocumentFileDto.CurrentChange> UploadProfileDocumentAsync(
        Guid id,
        UploadCompanyProfileFile.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<UploadCompanyProfileFile.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpDelete("{id:guid}/profile/documents/{documentFileId:guid}")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.CompanyInReview,
        ErrorCodes.CompanyDocumentFileNotFound,
        ErrorCodes.ParamsMissMatch
    )]
    public Task<Guid> DeleteProfileDocumentAsync(
        Guid id,
        Guid documentFileId,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new DeleteCompanyProfileFile.Request(id, documentFileId), cancellationToken);
    }

    [HttpGet("{id:guid}/profile/payment-type")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfilePaymentTypeDto> GetProfilePaymentTypeAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfilePaymentType.Request(id), cancellationToken);
    }

    [HttpPatch("{id:guid}/profile/request-credit")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInReview)]
    public Task RequestProfileCreditAsync(
        Guid id,
        RequestCompanyProfileCredit.BaseRequest request,
        CancellationToken cancellationToken
    )
    {
        var req = request.Adapt<RequestCompanyProfileCredit.Request>();
        req.CompanyId = id;

        return Mediator.Send(req, cancellationToken);
    }

    [HttpGet("{id:guid}/profile/agreements")]
    [OperationErrors(ErrorCodes.CompanyNotFound)]
    public Task<CompanyProfileAgreementsDto> GetProfileAgreementsAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetCompanyProfileAgreements.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/profile/send-sign-registration-email")]
    [OperationErrors(
        ErrorCodes.CompanyNotFound,
        ErrorCodes.CompanyRegistrationAlreadySigned,
        ErrorCodes.CompanyOwnerNotFound
    )]
    public Task SendProfileSignRegistrationEmailAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new SendCompanySignRegistrationEmail.Request(id), cancellationToken);
    }

    [HttpPost("{id:guid}/profile/send-to-review")]
    [OperationErrors(ErrorCodes.CompanyNotFound, ErrorCodes.CompanyInvalidStatus, ErrorCodes.CompanyHasNoChanges)]
    public Task<CompanyStatus> SendProfileChangesToReviewAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new SendCompanyProfileChangesToReview.Request(id), cancellationToken);
    }
}
