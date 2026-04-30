using WebShop.Domain.Enum;

namespace WebShop.Domain.Response
{
    /// <summary>
    /// Универсальный класс-обёртка для ответов от сервисного слоя
    /// Содержит данные, статус и описание ошибки
    /// </summary>
    /// <typeparam name="T">Тип возвращаемых данных</typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// Описание ошибки или информационное сообщение
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Код статуса выполнения операции
        /// </summary>
        public StatusCode StatusCode { get; set; }

        /// <summary>
        /// Данные, возвращаемые в ответе (может быть null при ошибке)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Флаг успешного выполнения операции
        /// </summary>
        public bool IsSuccess => 
            StatusCode == StatusCode.OK || 
            StatusCode == StatusCode.Created || 
            StatusCode == StatusCode.NoContent;
    }
}
