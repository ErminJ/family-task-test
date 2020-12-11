using Core.Extensions.ModelConversion;
using Domain.Commands;
using Domain.Queries;
using Domain.ViewModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebClient.Abstractions;

namespace WebClient.Services
{
    public class TaskDataService: ITaskDataService
    {
        private readonly HttpClient httpClient;
        public TaskDataService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FamilyTaskAPI");
            Tasks = new List<TaskVm>();
            LoadTasks();
        }


        public IEnumerable<TaskVm> Tasks { get; private set; }
        public TaskVm SelectedTask { get; private set; }


        public event EventHandler TasksUpdated;
        public event EventHandler TaskSelected;
        public event EventHandler<string> CreateTaskFailed;
        public event EventHandler<string> UpdateTaskFailed;
        public event EventHandler<string> UpdateTasksFailed;

        private async void LoadTasks()
        {
            Tasks = (await GetAllTasks()).Payload;
            TasksUpdated?.Invoke(this, null);
        }

        private async Task<CreateTaskCommandResult> Create(CreateTaskCommand command)
        {
            return await httpClient.PostJsonAsync<CreateTaskCommandResult>("tasks", command);
        }

        private async Task<GetAllTasksQueryResult> GetAllTasks()
        {
            return await httpClient.GetJsonAsync<GetAllTasksQueryResult>("tasks");
        }

        private async Task<UpdateTaskCommandResult> Update(UpdateTaskCommand command)
        {
            return await httpClient.PutJsonAsync<UpdateTaskCommandResult>($"tasks/{command.Id}", command);
        }

        public void SelectTask(TaskVm selectedTask)
        {
            SelectedTask = selectedTask;
            TaskSelected?.Invoke(this, null);
        }

        public async Task ToggleTask(Guid id)
        {
            foreach (var taskModel in Tasks)
            {
                if (taskModel.Id == id)
                {
                    taskModel.IsComplete = !taskModel.IsComplete;

                    var result = await Update(taskModel.ToUpdateTaskCommand());

                    Console.WriteLine(JsonSerializer.Serialize(result));

                    if (result!=null)
                    {
                        TasksUpdated?.Invoke(this, null);
                        return;
                    }                  
                    UpdateTaskFailed?.Invoke(this, "Unable to save changes.");
                }
            }
        }

        public async Task AssignMemberToTask(Guid memberId)
        {
            SelectedTask.AssignedMemberId = memberId;

            var result = await Update(SelectedTask.ToUpdateTaskCommand());

            Console.WriteLine(JsonSerializer.Serialize(result));

            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    Tasks = updatedList;
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
                UpdateTasksFailed?.Invoke(this, "The save was successful, but we can no longer get an updated list of members from the server.");
            }
             UpdateTaskFailed?.Invoke(this, "Unable to save changes.");
        }

        public async Task AddTask(TaskVm model)
        {
            var result = await Create(model.ToCreateTaskCommand());
            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload;

                if (updatedList != null)
                {
                    Tasks = updatedList;
                    TasksUpdated?.Invoke(this, null);
                    return;
                }
                UpdateTasksFailed?.Invoke(this, "The creation was successful, but we can no longer get an updated list of tasks from the server.");
            }

            CreateTaskFailed?.Invoke(this, "Unable to create record.");
        }
    }
}