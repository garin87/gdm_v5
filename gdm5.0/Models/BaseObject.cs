
using gdm5._0.Domain.Interfaces.Models;

namespace gdm5._0.Models
{
    public abstract class BaseObject: IDbSortingModel
    {
       public int Id { get; set; }
    }

}
