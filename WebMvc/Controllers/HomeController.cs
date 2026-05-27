using System.Diagnostics;
using DtsDevChallenge.Common;
using Microsoft.AspNetCore.Mvc;
using WebMvc.Models;

namespace DtsDevChallenge.WebMvc.Controllers;

public class HomeController(ApiClient apiClient) : Controller
{
    public async Task<IActionResult> Index()
    {
        var tasks = await apiClient.GetTaskList();
        return View(tasks);
    }
    
    [HttpGet("/{id}")]
    public async Task<IActionResult> Task(int id)
    {
        var task = await apiClient.GetTask(id);
        return View(task);
    }
    
    [HttpPost("/{id}")]
    public async Task<IActionResult> TaskUpdate(int id, TaskItem model)
    {
        if (ModelState.IsValid)
        {
            var task = await apiClient.UpdateTask(model);
            if (task == null)
            {
                return RedirectToAction("Task", new { id });
            }
        
            return RedirectToAction("Task", new { id = task.Id });
        }
        
        return View("Task", model);
    }
    
    [HttpPost, HttpGet]
    public async Task<IActionResult> TaskCreate(TaskItem model)
    {
        if (ModelState.IsValid)
        {
            var newTask = await apiClient.CreateTask(model);

            if (newTask == null)
            {
                return View("Task", model);
            }
            
            return RedirectToAction("Task", new { id = newTask.Id });
        }
        
        return View("Task", model);
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost("/{id}/delete")]
    public async Task<IActionResult> TaskDelete(int id)
    {
        await apiClient.DeleteTask(id);
        return RedirectToAction("Index");
    }
}