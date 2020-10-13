namespace Lab.EventSourcing.Core
{
    public interface IProjectorHost
    {
        void Add<TModel>(IProjector<TModel> synchronizer)
            where TModel : EventSourcingModel<TModel>;

        void InvokeProjector<TModel>(TModel model)
            where TModel : EventSourcingModel<TModel>;
    }
}
