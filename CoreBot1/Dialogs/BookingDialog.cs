// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.15.0

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System.Threading;
using System.Threading.Tasks;
using ParserTabletkiUa;


namespace CoreBot1.Dialogs
{
    public class BookingDialog : CancelAndHelpDialog
    {
        private const string DrugNameToSearch = "Введіть назву препарату:";
        private const string CityToSearch = "Місто пошуку?";

        public BookingDialog()
            : base(nameof(BookingDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DestinationStepAsync,
                OriginStepAsync,
                TravelDateStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            if (bookingDetails.DrugName == null)
            {
                var promptMessage = MessageFactory.Text(DrugNameToSearch, DrugNameToSearch, InputHints.ExpectingInput);

                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(bookingDetails.DrugName, cancellationToken);
        }

        private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            bookingDetails.DrugName = (string)stepContext.Result;
            // Відгут після ввода назви препарату
            string list  = "";
            if (bookingDetails.CityName == null)
            {
                Parser.Host = @"https://tabletki.ua/uk/";
                Parser.SearchByName(bookingDetails.DrugName);
                Thread.Sleep(500);

                long i = 0;
                while (true)
                {
                    if (Parser.IsFound)
                    {
                        Parser.IsFound = false;
                        foreach (DrugBlock db in Parser.ListOfDrugsBlocks)
                        {

                            list += db.ToString();
                            list += "\n";

                        }
                        break;
                    }
                    i++;
                    if (i > 10000000000)
                    {
                        list = "Time is out!";
                        break;
                    }
                }

                

                var promptMessage = MessageFactory.Text(list, list, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
              
                
            }

            return await stepContext.NextAsync(bookingDetails.CityName, cancellationToken);
        }

        private async Task<DialogTurnResult> TravelDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            bookingDetails.CityName = (string)stepContext.Result;

            if (bookingDetails.MySearchDate == null || IsAmbiguous(bookingDetails.MySearchDate))
            {
                return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), bookingDetails.MySearchDate, cancellationToken);
            }

            return await stepContext.NextAsync(bookingDetails.MySearchDate, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (BookingDetails)stepContext.Options;

            bookingDetails.MySearchDate = (string)stepContext.Result;

            var messageText = $"Please confirm, I have you traveling to: {bookingDetails.DrugName} from: {bookingDetails.CityName} on: {bookingDetails.MySearchDate}. Is this correct?";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                var bookingDetails = (BookingDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(bookingDetails, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
