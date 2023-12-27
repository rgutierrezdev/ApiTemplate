using System.Reflection;
using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Extensions;
using ApiTemplate.Application.Common.Pdf;
using ApiTemplate.Domain.Entities;
using ApiTemplate.Infrastructure.Common;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Markdown;
using QuestPDF.Previewer;

namespace ApiTemplate.Infrastructure.Pdf;

public class CompanyRegistrationPdf : ICompanyRegistrationPdf
{
    private readonly AppSettings _appSettings;
    private const string ColorGreenDark = "69775e";
    private const string ColorGreenLight = "b8c98b";
    private const int PaddingHorizontal = 15;

    private CompanyRegistrationPdfModel Model { get; set; } = default!;

    public CompanyRegistrationPdf(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public byte[] Generate(CompanyRegistrationPdfModel model)
    {
        Model = model;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });

            if (Model.Signature != null)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Content().Element(ComposeSignaturesPage);
                });
            }
        });

        // document.ShowInPreviewer();
        // return Array.Empty<byte>();

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.SemiBold().FontSize(28).FontColor(ColorGreenDark);
        var subTitleStyle = TextStyle.Default.Italic().FontSize(15).FontColor(ColorGreenDark);

        container
            .Background(ColorGreenLight)
            .Row(row =>
            {
                row.RelativeItem()
                    .PaddingTop(20)
                    .PaddingBottom(15)
                    .PaddingHorizontal(PaddingHorizontal)
                    .Column(column =>
                    {
                        column.Item().Text("REGISTRO").Style(titleStyle);
                        column.Item().Text("Información de Clientes").Style(subTitleStyle);

                        column.Item().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                            x.DefaultTextStyle(style => style.FontColor(ColorGreenDark).FontSize(10));
                        });
                    });

                row.RelativeItem(0.18f)
                    .AlignCenter()
                    .AlignMiddle()
                    .PaddingRight(15)
                    .Column(col =>
                    {
                        var logoLocation = Path.Join(
                            Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location),
                            @"Pdf\Assets\ApiTemplateMainLogo.png"
                        );

                        col.Item()
                            .Width(70)
                            .PaddingBottom(10)
                            .Image(logoLocation);

                        col.Item().AlignCenter()
                            .Text("ApiTemplate Colombia SAS")
                            .FontColor(ColorGreenDark)
                            .FontSize(8)
                            .SemiBold();

                        col.Item().AlignCenter()
                            .Text("901.556.234-1")
                            .FontColor(ColorGreenDark)
                            .FontSize(8);
                    });
            });
    }

    private void ComposeContent(IContainer container)
    {
        container
            .PaddingVertical(15)
            .Column(x =>
            {
                x.Spacing(10);

                AddSectionTitle(x.Item(), "SECCIÓN A", "Información sobre la empresa");

                x.Item()
                    .PaddingHorizontal(PaddingHorizontal)
                    .Column(column =>
                    {
                        column.Spacing(5);

                        column.Item()
                            .Text("Sobre la empresa")
                            .ExtraBold();

                        column.Item().Row(row =>
                        {
                            var personType = Model.PersonType switch
                            {
                                PersonType.Legal => "Persona Jurídica",
                                PersonType.Natural => "Personal Natural",
                                _ => ""
                            };

                            AddFieldColumn(row.RelativeItem(0.4f), "Tipo de persona", personType);
                            AddFieldColumn(row.RelativeItem(), "Nombre o razón social", Model.LegalName ?? "");
                        });


                        column.Item().Row(row =>
                        {
                            AddFieldColumn(
                                row.RelativeItem(0.4f),
                                "Tipo de identificacion",
                                Model.DocumentTypeName ?? ""
                            );
                            AddFieldColumn(
                                row.RelativeItem(),
                                "Número de identificación",
                                $"{Model.Document ?? ""}-{Model.VerificationDigit ?? ""}"
                            );
                        });
                    });

                if (Model.PersonType == PersonType.Legal)
                {
                    x.Item()
                        .PaddingHorizontal(PaddingHorizontal)
                        .Column(column =>
                        {
                            column.Spacing(5);

                            column.Item()
                                .Text("Sobre su representante legal")
                                .ExtraBold();

                            column.Item().Row(row =>
                            {
                                AddFieldColumn(
                                    row.RelativeItem(0.4f),
                                    "Nombres",
                                    Model.LegalRepresentativeFirstName ?? ""
                                );
                                AddFieldColumn(row.RelativeItem(), "Apellidos",
                                    Model.LegalRepresentativeLastName ?? "");
                            });

                            column.Item().Row(row =>
                            {
                                AddFieldColumn(
                                    row.RelativeItem(0.8f),
                                    "Tipo de identificacion",
                                    Model.LegalRepresentativeDocumentTypeName ?? ""
                                );
                                AddFieldColumn(
                                    row.RelativeItem(),
                                    "Número de identificación",
                                    Model.LegalRepresentativeDocument ?? ""
                                );
                                AddFieldColumn(
                                    row.RelativeItem(),
                                    "Correo electrónico",
                                    Model.LegalRepresentativeEmail ?? ""
                                );
                            });
                        });
                }

                AddSectionTitle(x.Item(), "SECCIÓN B", "Información sobre los socios");

                x.Item().PaddingHorizontal(PaddingHorizontal).Column(column =>
                {
                    column.Spacing(5);

                    column.Item().Text(
                            "Esta información es de los socios o accionistas que sean propietarios, directa o indirectamente de más del 5% del capital"
                        )
                        .Bold();

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                            cols.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Nombre Completo").Bold().FontSize(10);
                            header.Cell().Text("Identificación").Bold().FontSize(10);
                            header.Cell().Text("% de participación").Bold().FontSize(10);
                            header.Cell().Text("¿Es una PEP?").Bold().FontSize(10);
                        });

                        foreach (var associate in Model.Associates)
                        {
                            table.Cell().Text(associate.Name).Thin();
                            table.Cell().Text($"{associate.DocumentTypeShortName}: {associate.Document}").Thin();
                            table.Cell().Text(associate.ParticipationPercent.ToString()).Thin();
                            table.Cell().Text(GetYesNoValue(associate.Pep)).Thin();
                        }
                    });
                });

                x.Item()
                    .Background(ColorGreenDark)
                    .PaddingHorizontal(PaddingHorizontal)
                    .PaddingVertical(15)
                    .Column(column =>
                    {
                        column.Spacing(5);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Text(
                                    "¿Tiene algún familiar hasta el segundo grado de consanguinidad segundo de afinidad y primero civil o un socio que ostente la calidad de PEP?"
                                )
                                .FontColor(Colors.White)
                                .FontSize(10);

                            row.RelativeItem(0.1f).Text(GetYesNoValue(Model.HasPepRelative)).Bold();
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem()
                                .Text(
                                    "¿Declara bajo gravedad de juramento que los recursos aportados no son del estado colombiano o del extranjero?"
                                )
                                .FontColor(Colors.White)
                                .FontSize(10);

                            row.RelativeItem(0.1f).Text(GetYesNoValue(Model.UnderOath)).Bold();
                        });
                    });

                AddSectionTitle(x.Item(), "SECCIÓN C", "Declaraciones y autorizaciones");

                x.Item()
                    .PaddingHorizontal(PaddingHorizontal)
                    .Row(row =>
                    {
                        var url = _appSettings.FrontEndBaseUrl + "/contextual-documents/viewer/" +
                                  ContextualDocumentType.PrivacyPolicy.ToString().ToCamelCase();

                        row.RelativeItem().Column(column =>
                        {
                            column.Spacing(5);

                            column.Item().Markdown(
                                "Con la firma del presente documento autorizo a **ApiTemplate Colombia SAS** como responsable del tratamiento de los datos personales obtenidos en el presente formulario y de aquellos que surjan de la presente relación comercial"
                            );

                            column.Item().Hyperlink(url).Text(url);
                        });

                        var image = GenerateQrLink(url);

                        row.RelativeItem(0.15f)
                            .AlignCenter()
                            .AlignMiddle()
                            .Padding(-10)
                            .ScaleToFit()
                            .Image(image);
                    });

                x.Item()
                    .Background(ColorGreenLight)
                    .PaddingHorizontal(PaddingHorizontal)
                    .Row(row =>
                    {
                        row.RelativeItem().Text("ACEPTO Y AUTORIZO EL TRATAMIENTO DE LOS DATOS PERSONALES")
                            .Bold();

                        row.RelativeItem(0.1f).Text(GetYesNoValue(true)).Bold();
                    });

                x.Item()
                    .PaddingHorizontal(PaddingHorizontal)
                    .Row(row =>
                    {
                        row.RelativeItem().Markdown(
                            "Declaro que los recursos utilizados o a utilizarse en la relación comercial con **ApiTemplate Colombia SAS**, provienen de actividades lícitas; Manifiesto que aquellos no son resultado de actividades penalizadas por el ordenamiento colombiano. Declaro bajo la gravedad de juramento que ni yo ni la sociedad que represento, los demás representantes legales de la misma ni sus accionistas, actualmente nos encontramos incluidos en ninguna lista vinculante para Colombia, no hemos sido vinculados a investigación alguna ante cualquier autoridad como resultado de investigaciones en procesos de extinción de dominio, no hemos sido condenados, y no se ha emitido en nuestra contra sentencia o fallo en relación con las conductas mencionadas en este párrafo."
                        );
                    });

                x.Item()
                    .PaddingHorizontal(PaddingHorizontal)
                    .Row(row =>
                    {
                        var contextualDocType = Model.Type == CompanyType.Customer
                            ? ContextualDocumentType.CustomerTermsAndConditions
                            : ContextualDocumentType.VendorTermsAndConditions;

                        var url = _appSettings.FrontEndBaseUrl + "/contextual-documents/viewer/" +
                                  contextualDocType.ToString().ToCamelCase();

                        row.RelativeItem().Column(column =>
                        {
                            column.Spacing(5);

                            column.Item().Markdown(
                                "Declaro que he leído y he aceptado los términos y condiciones encontrados en la plataforma, de la relación comercial con **ApiTemplate Colombia SAS.**"
                            );

                            column.Item().Hyperlink(url).Text(url);
                        });

                        var image = GenerateQrLink(url);

                        row.RelativeItem(0.15f)
                            .AlignCenter()
                            .AlignMiddle()
                            .Padding(-10)
                            .ScaleToFit()
                            .Image(image);
                    });

                x.Item()
                    .Background(ColorGreenLight)
                    .PaddingHorizontal(PaddingHorizontal)
                    .Row(r =>
                    {
                        r.RelativeItem().Text("ACEPTO LA TOTALIDAD DE LOS TÉRMINOS Y CONDICIONES")
                            .Bold();

                        r.RelativeItem(0.1f).Text(GetYesNoValue(Model.AgreesTermsAndConditions)).Bold();
                    });

                x.Item()
                    .PaddingHorizontal(PaddingHorizontal)
                    .Row(row =>
                    {
                        row.RelativeItem().Markdown(
                            "Acepto y autorizo a **ApiTemplate Colombia SAS** a consultar, en cualquier tiempo y en cualquier base de datos manejada por un operador de información financiera y crediticia, toda la información relevante para conocer mi desempeño como deudor o de la empresa a la cual represento, entre los cuales está: capacidad de pago, la viabilidad para entablar o mantener una relación contractual, o para cualquier otra finalidad, incluyendo sin limitarse la realización de campañas de mercadeo, ofrecimiento de productos y publicidad en general. Adicionalmente autorizo a **ApiTemplate Colombia SAS** a reportar a cualquier base de datos manejada por un operador, datos, tratados o sin tratar, sobre el cumplimiento o incumplimiento de mis obligaciones crediticias, mis deberes legales de contenido patrimonial, mis datos de ubicación y contacto (número de teléfono fijo, número de teléfono celular, dirección del domicilio, dirección laboral y correo electrónico), mis solicitudes de crédito así como otros atinentes a mis relaciones comerciales, financieras y en general socioeconómicas que haya entregado o que consten en registros públicos, bases de datos públicas o documentos públicos."
                        );
                    });

                x.Item()
                    .Background(ColorGreenLight)
                    .PaddingHorizontal(PaddingHorizontal)
                    .Row(r =>
                    {
                        r.RelativeItem()
                            .Text("ACEPTO Y AUTORIZO REVISIONES EN LISTAS VINCULANTES Y FINANCIERAS")
                            .Bold();

                        r.RelativeItem(0.1f).Text(GetYesNoValue(Model.AuthorizesFinancialInformation)).Bold();
                    });

                AddSectionTitle(x.Item(), "SECCIÓN D", "Información operativa");

                var contactRole = Model.Type switch
                {
                    CompanyType.Customer => "compras",
                    CompanyType.Vendor => "ventas",
                    _ => ""
                };

                AddContact(x.Item(), "Sobre la persona de contacto principal", CompanyContactType.Main);
                AddContact(x.Item(), "Sobre la persona encargada de tesoreria", CompanyContactType.Treasury);
                AddContact(x.Item(),
                    "Sobre la persona encargada de " + contactRole,
                    CompanyContactType.SalesPurchasing
                );

                if (Model.Type == CompanyType.Customer)
                {
                    x.Item()
                        .Background(ColorGreenLight)
                        .PaddingHorizontal(PaddingHorizontal)
                        .PaddingVertical(2)
                        .Column(column =>
                        {
                            column.Spacing(5);

                            column.Item()
                                .Text("Sobre la facturación electrónica")
                                .ExtraBold();

                            column.Item().Row(row =>
                            {
                                AddFieldColumn(row.RelativeItem(1.5f), "Nombre", Model.EInvoiceFullName ?? "");
                                AddFieldColumn(row.RelativeItem(), "Correo electrónico", Model.EInvoiceEmail ?? "");
                                AddFieldColumn(row.RelativeItem(), "Teléfono", Model.EInvoicePhoneNumber ?? "");
                            });

                            column.Item().Row(row =>
                            {
                                AddFieldColumn(row.RelativeItem(),
                                    "Día de cierre contable mensual",
                                    Model.EInvoicePhoneNumber ?? ""
                                );
                            });
                        });
                }

                x.Item()
                    .PaddingHorizontal(PaddingHorizontal)
                    .Column(column =>
                    {
                        column.Spacing(5);

                        column.Item()
                            .Text("Sobre lo tributario y fiscal")
                            .ExtraBold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(0.1f);
                                cols.RelativeColumn();

                                cols.RelativeColumn(0.1f);
                                cols.RelativeColumn();
                            });

                            table.Cell().Text(GetYesNoValue(Model.RetentionSubject)).ExtraBold();
                            table.Cell().Text("Sujeto de retención");

                            table.Cell().Text(GetYesNoValue(Model.RequiredToDeclareIncome)).ExtraBold();
                            table.Cell().Text("Obligado a declarar renta");

                            table.Cell().Text(GetYesNoValue(Model.VatResponsible)).ExtraBold();
                            table.Cell().Text("Responsable de IVA");

                            table.Cell().Text(GetYesNoValue(Model.DianGreatContributor)).ExtraBold();
                            table.Cell().Text("Gran contribuyente");

                            table.Cell().Text(GetYesNoValue(Model.SalesRetentionAgent)).ExtraBold();
                            table.Cell().Text("Agente de retención en ventas");

                            table.Cell().Text(GetYesNoValue(Model.IncomeSelfRetainer)).ExtraBold();
                            table.Cell().Text("Autorretenedor de renta");

                            table.Cell().Text(GetYesNoValue(Model.IcaAutoRetainer)).ExtraBold();
                            table.Cell().Text("Autorretenedor ICA");
                        });

                        column.Item()
                            .Row(row =>
                            {
                                AddFieldColumn(row.RelativeItem(), "Actividad ICA", Model.IcaActivity ?? "");

                                var regime = Model.Regime switch
                                {
                                    CompanyRegime.VatResponsible => "Régimen responsable de IVA",
                                    CompanyRegime.NoVatResponsible => "Régimen NO responsable de IVA",
                                    CompanyRegime.Special => "Régimen tributario especial",
                                    CompanyRegime.Simple => "Régimen tributario simple",
                                    _ => ""
                                };

                                AddFieldColumn(row.RelativeItem(), "Régimen", regime);
                            });
                    });
            });
    }

    private void ComposeFooter(IContainer container)
    {
        container
            .BorderTop(6)
            .BorderColor(ColorGreenLight)
            .AlignRight()
            .Row(row =>
            {
                var logoLocation = Path.Join(
                    Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location),
                    @"Pdf\Assets\ApiTemplateLogo.png"
                );

                row.ConstantItem(150)
                    .PaddingVertical(8)
                    .PaddingRight(8)
                    .Image(logoLocation);
            });
    }

    private void ComposeSignaturesPage(IContainer container)
    {
        container.Column(x =>
        {
            x.Item().Row(row =>
            {
                row.RelativeItem()
                    .PaddingTop(40)
                    .AlignCenter()
                    .Text("Firmas Electrónicas")
                    .Bold()
                    .FontSize(15);
            });

            x.Item()
                .PaddingTop(40)
                .PaddingHorizontal(30)
                .BorderBottom(1)
                .BorderColor("eee")
                .PaddingBottom(30)
                .Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        var url = _appSettings.FrontEndBaseUrl + "/signature-verification/" +
                                  Model.Signature?.SignedFileId;

                        row.RelativeItem().Column(column =>
                        {
                            var image = GenerateQrLink(url);

                            column.Item()
                                .Padding(-15)
                                .PaddingRight(-20)
                                .Width(150)
                                .Image(image);
                        });

                        row.RelativeItem(2.8f).Column(column =>
                        {
                            column.Spacing(10);

                            column.Item().Row(r =>
                            {
                                AddFieldColumn(r.RelativeItem(),
                                    "Id del Documento",
                                    Model.Signature?.SignedFileId.ToString() ?? "");
                            });

                            column.Item().Row(r =>
                            {
                                AddFieldColumn(r.RelativeItem(),
                                    "Nombre del Documento",
                                    Model.Signature?.FileName ?? "");
                            });

                            column.Item().Row(r =>
                            {
                                AddFieldColumn(r.RelativeItem(),
                                    "URL de Verificación",
                                    url);
                            });
                        });
                    });
                });

            x.Item()
                .PaddingTop(40)
                .PaddingHorizontal(30)
                .Column(col =>
                {
                    col.Spacing(5);

                    col.Item().Row(row =>
                    {
                        AddFieldColumn(row.RelativeItem(),
                            "Nombre Completo",
                            Model.Signature?.FullName ?? "");

                        AddFieldColumn(row.RelativeItem(),
                            "Documento",
                            (Model.Signature?.DocumentTypeShortName ?? "") +
                            (Model.Signature?.Document ?? "")
                        );
                    });

                    col.Item().Row(row =>
                    {
                        AddFieldColumn(row.RelativeItem(),
                            "Correo electrónico",
                            Model.Signature?.Email ?? "");

                        AddFieldColumn(row.RelativeItem(),
                            "Firmado electrónicamente el",
                            Model.Signature?.SignedDate.ToString() ?? "");
                    });

                    col.Item().Row(row =>
                    {
                        AddFieldColumn(row.RelativeItem(),
                            "Dirección IP",
                            Model.Signature?.IpAddress ?? "");

                        AddFieldColumn(row.RelativeItem(),
                            "Token",
                            Model.Signature?.Token ?? "");
                    });

                    col.Item().Row(row =>
                    {
                        AddFieldColumn(row.RelativeItem(),
                            "Cliente",
                            Model.Signature?.Client ?? "");
                    });
                });
        });
    }

    private void AddSectionTitle(IContainer container, string title, string subtitle)
    {
        var titleStyle = TextStyle.Default.Bold().FontSize(23).FontColor(Colors.White);
        var subtitleStyle = TextStyle.Default.Thin().FontSize(18).FontColor(Colors.White);

        container
            .Background(ColorGreenDark)
            .PaddingHorizontal(PaddingHorizontal)
            .Row(row =>
            {
                row.RelativeItem(0.35f)
                    .AlignMiddle()
                    .Text(title)
                    .Style(titleStyle);

                row.RelativeItem()
                    .AlignMiddle()
                    .Text(subtitle)
                    .Style(subtitleStyle);
            });
    }

    private void AddFieldColumn(IContainer container, string label, string value)
    {
        container.Column(column =>
        {
            column.Item().Text(label).Bold().FontSize(10);
            column.Item().Text(value).Thin();
        });
    }

    private void AddContact(IContainer container, string title, CompanyContactType contactType)
    {
        container
            .PaddingHorizontal(PaddingHorizontal)
            .Column(column =>
            {
                column.Spacing(5);

                column.Item()
                    .Text(title)
                    .ExtraBold();

                var contact = Model.Contacts.FirstOrDefault(c => c.Type == contactType);

                column.Item().Row(row =>
                {
                    AddFieldColumn(row.RelativeItem(1.5f), "Nombre", contact?.Name ?? "");
                    AddFieldColumn(row.RelativeItem(), "Correo electrónico", contact?.Email ?? "");
                    AddFieldColumn(row.RelativeItem(), "Teléfono", contact?.PhoneNumber ?? "");
                });
            });
    }

    private string GetYesNoValue(bool? value)
    {
        return value switch
        {
            true => "SI",
            false => "NO",
            _ => "__"
        };
    }

    private byte[] GenerateQrLink(string url)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new BitmapByteQRCode(qrCodeData);

        return qrCode.GetGraphic(20);
    }
}
