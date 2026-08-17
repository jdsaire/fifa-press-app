using FifaPressApp.Models;

namespace FifaPressApp.Services;

/// <summary>
/// The narrative text for the two changes this app writes at runtime, in all
/// three languages.
///
/// <para>
/// <b>Authored here rather than looked up from the UI resource files, and the
/// distinction is the one <c>11_I18N.md</c> §4.2 draws.</b> The per-locale JSON
/// holds interface strings — labels, headings, buttons. This is Category D:
/// record content, in the same standing as the eight seeded changes, which are
/// authored in three languages in the source because that is what a real
/// accreditation system would do at the point the change is written. Pulling
/// this out of the UI dictionary would file record content as interface
/// chrome.
/// </para>
///
/// <para>
/// <b>Written in all three at write time, not resolved at render time.</b> A
/// change requested while the app is in Spanish still reads correctly after a
/// switch to Portuguese, because all three were authored the moment it was
/// created. A change that stored only the language it was made in would be a
/// record that quietly disagrees with the rest of the log.
/// </para>
/// </summary>
internal static class ChangeTemplates
{
    public static LocalizedText RequestWhatChanged(int matchNumber) => new(
        En: $"Access to match {matchNumber} is now recorded as requested.",
        Es: $"El acceso al partido {matchNumber} queda registrado como solicitado.",
        Pt: $"O acesso ao jogo {matchNumber} fica registado como solicitado.");

    public static LocalizedText RequestReason { get; } = new(
        En: "You submitted a request from the match page. It is written to your record "
          + "before any decision is taken, so a request in progress is never invisible.",
        Es: "Enviaste una solicitud desde la página del partido. Queda escrita en tu registro "
          + "antes de que se tome ninguna decisión, para que una solicitud en curso nunca sea "
          + "invisible.",
        Pt: "Enviou um pedido a partir da página do jogo. Fica escrito no seu registo antes de "
          + "ser tomada qualquer decisão, para que um pedido em curso nunca seja invisível.");

    public static LocalizedText RequestNextStep { get; } = new(
        En: "Nothing to do now. When a decision is taken it appears here as its own "
          + "change, with the reason attached.",
        Es: "No hay nada que hacer por ahora. Cuando se tome una decisión aparecerá aquí como "
          + "un cambio propio, con el motivo adjunto.",
        Pt: "Não há nada a fazer por agora. Quando for tomada uma decisão, aparecerá aqui como "
          + "uma alteração própria, com o motivo anexado.");

    public static LocalizedText RequestDecidedBy { get; } = new(
        En: "FIFA Event Media Operations (simulated — no request is actually sent)",
        Es: "FIFA Event Media Operations (simulado: no se envía ninguna solicitud)",
        Pt: "FIFA Event Media Operations (simulado: nenhum pedido é realmente enviado)");

    public static LocalizedText WithdrawalWhatChanged(int? matchNumber) => matchNumber is int match
        ? new LocalizedText(
            En: $"Your request for access to match {match} is withdrawn.",
            Es: $"Tu solicitud de acceso al partido {match} queda retirada.",
            Pt: $"O seu pedido de acesso ao jogo {match} fica retirado.")
        : new LocalizedText(
            En: "Your request is withdrawn.",
            Es: "Tu solicitud queda retirada.",
            Pt: "O seu pedido fica retirado.");

    public static LocalizedText WithdrawalReason { get; } = new(
        En: "You withdrew this request yourself. The original request stays in the record "
          + "below, because a record that erases what it replaces is not a record.",
        Es: "Retiraste esta solicitud tú mismo. La solicitud original sigue en el registro de "
          + "abajo, porque un registro que borra lo que reemplaza no es un registro.",
        Pt: "Retirou este pedido você mesmo. O pedido original permanece no registo abaixo, "
          + "porque um registo que apaga aquilo que substitui não é um registo.");

    public static LocalizedText WithdrawalNextStep { get; } = new(
        En: "You can request access to this match again at any time before kickoff.",
        Es: "Puedes volver a solicitar acceso a este partido en cualquier momento antes del "
          + "inicio.",
        Pt: "Pode voltar a solicitar acesso a este jogo a qualquer momento antes do pontapé de "
          + "saída.");
}
