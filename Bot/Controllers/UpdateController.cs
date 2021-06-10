using System.Threading.Tasks;
using Bot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot.Controllers
{
    [Route("update")]
    public class UpdateController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ICallbackQueryService _callbackQueryService;

        public UpdateController(IMessageService messageService, ICallbackQueryService callbackQueryService)
        {
            _messageService = messageService;
            _callbackQueryService = callbackQueryService;
        }

        public async Task<IActionResult> PostAsync([FromBody] Update update)
        {
            if (update.Type == UpdateType.Message)
            {
                await _messageService.HandleAsync(update.Message);
            }
            
            if (update.Type == UpdateType.CallbackQuery)
            {
                await _callbackQueryService.HandleAsync(update.CallbackQuery);
            }

            return Ok();
        }
    }
}
