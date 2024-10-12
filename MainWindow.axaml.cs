using Agents.Models;
using Avalonia.Controls;
using Avalonia.Media;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agents
{
    public partial class MainWindow : Window
    {
        public int amountOnPage = 10;
        public int currPage;
        public int allPages => (agentslist.Count + amountOnPage - 1) / amountOnPage;
        public List<Agent> agentsOnPage = new List<Agent>();
        public List<Agent> agentslist = Helper.myprojContext.Agents.Include(x => x.IdPriorityNavigation).Include(x => x.IdTypeNavigation).Include(x => x.IdDiscountNavigation).
            Include(x => x.Sales).Include(x => x.IdTochkaPrs).ToList();
        public MainWindow()
        {
            InitializeComponent();
            updateListBox();
        }

        public void updateListBox()
        {
            List<Agent> agents = Helper.myprojContext.Agents.Include(x => x.IdPriorityNavigation).Include(x => x.IdTypeNavigation).Include(x => x.IdDiscountNavigation).
            Include(x => x.Sales).Include(x => x.IdTochkaPrs).ToList();
            foreach (Agent agent in agents)
            {
                agent.IdDiscountNavigation = Helper.myprojContext.Discounts.Where(x => x.NameDisc == GetDiscount(agent.allAmount)).FirstOrDefault();
                agent.IdDiscount = agent.IdDiscountNavigation.IdDiscount;
            }
            if (typesCmb.SelectedIndex != -1 && typesCmb.SelectedIndex != 2)
            {
                int indType = typesCmb.SelectedIndex + 1;
                agents = agents.Where(x => x.IdType == indType).ToList();
            }
            
            switch (sortsCmb.SelectedIndex)
            {
                case 0:
                    agents = agents.OrderBy(x => x.Name).ToList();
                    break;
                case 1:
                    agents = agents.OrderByDescending(x => x.Name).ToList();
                    break;
                case 2:
                    agents = agents.OrderBy(x => x.IdDiscount).ToList();
                    break;
                case 3:
                    agents = agents.OrderByDescending(x => x.IdDiscount).ToList();
                    break;
                case 4:
                    agents = agents.OrderBy(x => x.IdPriority).ToList();
                    break;
                case 5:
                    agents = agents.OrderByDescending(x => x.IdPriority).ToList();
                    break;
                case 6:
                default:
                    break;
            }

            string searchText = search.Text ?? "";
            int count = searchText.Split(' ').Length;
            string[] values = new string[count];

            values = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in values)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    agents = agents.Where(x => x.Name.Contains(s)
                    || x.Email.Contains(s)|| x.Phone.Contains(s)).ToList();
                }
                else
                {
                    continue;
                }
            }

            if (agents.Count() > 10)
            {
                if (currPage == 0) 
                {
                    currPage = 1;
                }

                agentsOnPage.Clear();
                var list = agents;
                foreach (var agent in list.Skip((currPage - 1) * amountOnPage).Take(amountOnPage))
                {
                    agentsOnPage.Add(agent);
                }
                agentsList.ItemsSource = agentsOnPage.ToList();
            }
            else
            {
                prevBtn.IsVisible = false;
                nextBtn.IsVisible = false;
                var list = agents;
                agentsList.ItemsSource = list;
            }
        }
        private string GetDiscount(int amount)
        {
            if (amount < 10000)
                return "0";
            else if (amount >= 10000 && amount < 50000)
                return "5";
            else if (amount >= 50000 && amount < 150000)
                return "10";
            else if (amount >= 150000 && amount < 500000)
                return "20";
            else return "25"; 
        }

        private void Prev_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (currPage > 1)
            {
                currPage--;
            }
            updateListBox();
        }

        private void Next_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (currPage < allPages)
            {
                currPage++;
            }
            updateListBox();
        }

            private void Add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AddAgent addAgent = new AddAgent();
            addAgent.Show();
            this.Close();
        }

        private void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            int ind = (int)((sender as Button)!).Tag!;
            var agent = Helper.myprojContext.Agents.FirstOrDefault(x => x.IdAgent == ind);
            if (agent.Sales.Count == 0)
            {
                List<AgPriorChange> prCh = Helper.myprojContext.AgPriorChanges.Where(x => x.IdAgent == agent.IdAgent).ToList();
                List<TochkaProd> tochkaProds = agent.IdTochkaPrs.ToList();
                agent.IdTochkaPrs.Clear();
                Helper.myprojContext.AgPriorChanges.RemoveRange(prCh);
                Helper.myprojContext.SaveChanges();
                Helper.myprojContext.Agents.Remove(agent);
                Helper.myprojContext.SaveChanges();
            }
            else 
            {
                var msg = MessageBoxManager.GetMessageBoxStandard("Уведомление", "Нельзя удалить агента, так как у него есть история реализации");
                msg.ShowAsync();
            }
            updateListBox();
        }
        

        private void ListBox_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            var agent = agentsList.SelectedItem as Agent;
            AddAgent addAgent = new AddAgent(agent!);
            addAgent.Show();
            this.Close();
        }

        private void TextBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            updateListBox();
        }

        private void Sort_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
           updateListBox();
        }

        private void Filter_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            updateListBox();
        }
    }
}
