namespace Lab.EventSourcing.Core
{
    public interface IProjector<TModel> 
        where TModel : EventSourcingModel<TModel>
    {
        void Execute(TModel model);
    }
}
