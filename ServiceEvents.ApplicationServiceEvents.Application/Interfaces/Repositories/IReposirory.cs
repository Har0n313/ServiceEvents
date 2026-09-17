namespace ServiceEvents.Application.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Получить сущность по её уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Экземпляр сущности или null, если она не найдена.</returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую сущность в хранилище.
    /// </summary>
    /// <param name="entity">Экземпляр сущности для добавления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Асинхронная задача.</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить существующую сущность в хранилище.
    /// </summary>
    /// <param name="entity">Экземпляр сущности для обновления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Асинхронная задача.</returns>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить сущность из хранилища по идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор сущности.</param>
    /// <param name="entity">Экземпляр сущности для удаления</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Асинхронная задача.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}