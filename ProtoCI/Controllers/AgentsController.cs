using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ninject;
using ProtoCI.Core;
using ProtoCI.Core.Entities;
using ProtoCI.Core.Protocols;
using ProtoCI.DAL;
using ProtoCI.Models;

namespace ProtoCI.Controllers
{
    public class AgentsController : Controller
    {
        private readonly IProtocol _sshProtocol;
        private readonly IProtocol _winrmProtocol;

        private AgentContext db = new AgentContext();

        public AgentsController(
            [Named("SSH")] IProtocol sshProtocol,
            [Named("WinRM")] IProtocol winrmProtocol)
        {
            _sshProtocol = sshProtocol;
            _winrmProtocol = winrmProtocol;
        }

        // GET: Agents
        public async Task<ActionResult> Index()
        {
            return View(await db.Agents.ToListAsync());
        }

        // GET: Agents/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = await db.Agents.FindAsync(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // GET: Agents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AgentViewModel agentViewModel)
        {
            if (ModelState.IsValid)
            {
                if (agentViewModel.PrivateKeyFile != null)
                {
                    var target = new MemoryStream();
                    agentViewModel.PrivateKeyFile.InputStream.CopyTo(target);
                    agentViewModel.AgentData.PrivateKeyBytes = target.ToArray();
                }

                db.Agents.Add(agentViewModel.AgentData);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(agentViewModel);
        }

        // GET: Agents/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = await db.Agents.FindAsync(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(new AgentViewModel { AgentData = agent });
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AgentViewModel agentViewModel)
        {
            if (ModelState.IsValid)
            {
                if (agentViewModel.PrivateKeyFile != null)
                {
                    var target = new MemoryStream();
                    agentViewModel.PrivateKeyFile.InputStream.CopyTo(target);
                    agentViewModel.AgentData.PrivateKeyBytes = target.ToArray();
                }

                db.Entry(agentViewModel.AgentData).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(agentViewModel);
        }

        // GET: Agents/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = await db.Agents.FindAsync(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Agent agent = await db.Agents.FindAsync(id);
            db.Agents.Remove(agent);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task<ActionResult> Test(int id)
        {
            var agent = await db.Agents.FindAsync(id);

            var model = new AgentTestViewModel();
            try
            {
                var protocol = agent.PlatformType == AgentPlatformType.Windows
                    ? _winrmProtocol
                    : _sshProtocol;

                var conn = protocol.OpenConnection(agent);
                string output;
                try
                {
                    var commandText = agent.PlatformType == AgentPlatformType.Windows
                        ? "$env:COMPUTERNAME"
                        : "uname -a";

                    var cmd = protocol.StartCommand(conn, commandText, ".");
                    using (var reader = new StreamReader(cmd.StandardOutput))
                    {
                        output = reader.ReadToEnd();
                    }
                }
                finally
                {
                    protocol.CloseConnection(agent);
                }

                model.SystemIdentifier = output;
            }
            catch (Exception ex)
            {
                model.Exception = ex;
            }

            return View(model);
        }
    }
}
