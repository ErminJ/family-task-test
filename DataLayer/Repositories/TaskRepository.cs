using Core.Abstractions.Repositories;
using Domain.DataModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DataLayer
{
    public class TaskRepository : BaseRepository<Guid, Task, TaskRepository>, ITaskRepository
    {
        public TaskRepository(FamilyTaskContext context) : base(context)
        { }

        ITaskRepository IBaseRepository<Guid, Task, ITaskRepository>.NoTrack()
        {
            return base.NoTrack();
        }

        ITaskRepository IBaseRepository<Guid, Task, ITaskRepository>.Reset()
        {
            return base.Reset();
        }

        ITaskRepository IBaseRepository<Guid, Task, ITaskRepository>.Include(Expression<Func<Task, object>> includeExpression)
        {
            return base.Include(includeExpression);
        }

    }
}
