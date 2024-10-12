using Agents.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Agents;

public partial class AddAgent : Window
{
    public bool prChanged= false;
    public bool result = true;
    public List<string> types = Helper.myprojContext.Types.Select(x => x.NameType).ToList();
    public List<string> priorities = Helper.myprojContext.Priorities.Select(x => x.NamePr).ToList();
    public List<string> prods = Helper.myprojContext.Products.Select(x => x.NameProduct).ToList();
    public Agent agentEdit;
    public string path;
    public string destPath;
    public Bitmap bitmapToBind;
    public string filename;
    public string resultPhoto;

    public AddAgent()
    {
        InitializeComponent();
        agentEdit = new Agent();
       
        typeAgCmb.ItemsSource = types;
        priorAgCmb.ItemsSource = priorities;
        prodCmb.ItemsSource = prods;
    }
    public AddAgent(Agent agent)
    {
        agentEdit = agent;
        InitializeComponent();
        img.Source = agentEdit.bitmap;
        typeAgCmb.ItemsSource = types;
        priorAgCmb.ItemsSource = priorities;
        nameComp.Text = agentEdit.Name;
        addressComp.Text = agentEdit.Address;
        fioDirComp.Text = agentEdit.Director;
        phoneComp.Text = agentEdit.Phone;
        mailComp.Text = agentEdit.Email;
        innComp.Text = agentEdit.Inn.ToString();
        kppComp.Text = agentEdit.Kpp.ToString();
        priorAgCmb.SelectedItem = Helper.myprojContext.Priorities.Where(x => x.IdPriority == agentEdit.IdPriority).FirstOrDefault().NamePr;
        typeAgCmb.SelectedItem = Helper.myprojContext.Types.Where(x => x.IdType == agentEdit.IdType).FirstOrDefault().NameType;
        List<Sale> sales = Helper.myprojContext.Sales.Where(x => x.IdAgent == agentEdit.IdAgent).Include(x => x.IdProductNavigation).ToList();
        salesList.ItemsSource = sales;
        prodCmb.ItemsSource = prods;
    }
    private void TextBox_KeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        string searchText = search.Text ?? "";
        if (searchText.Length == 0)
        {
            salesList.ItemsSource = agentEdit.Sales;
        }
        else
        {
            List<Sale> salesSearch = new List<Sale>();
            int count = searchText.Split(' ').Length;
            string[] values = new string[count];

            values = searchText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in values)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    salesSearch = agentEdit.Sales.Where(x => x.IdProductNavigation.NameProduct.Contains(s)).ToList();
                }
                else
                {
                    continue;
                }
            }
            salesList.ItemsSource = salesSearch;
        }
        
    }

        private void Ok_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        result = true;
        string inn = innComp.Text;
        switch (checkFio(fioDirComp.Text))
        {
            case true:
                agentEdit.Director = fioDirComp.Text;
                break;
            case false:
                var msg = MessageBoxManager.GetMessageBoxStandard("Ошибка", "ФИО директора содержит недопустимые символы");
                msg.ShowAsync();
                result = false;
                break;
        }

        switch (checkInn(inn))
        {
            case true:
                agentEdit.Inn = Convert.ToInt64(inn);
                break;
            case false:
                var msg = MessageBoxManager.GetMessageBoxStandard("Ошибка", "ИНН содержит недопустимые символы");
                msg.ShowAsync();
                result = false;
                break;
        }
        string kpp = kppComp.Text;
        switch (checKpp(kpp))
        {
            case true:
                agentEdit.Kpp = Convert.ToInt64(kpp);
                break;
            case false:
                var msg = MessageBoxManager.GetMessageBoxStandard("Ошибка", "ИНН содержит недопустимые символы");
                msg.ShowAsync();
                result = false;
                break;
        }
        switch (checkMail(mailComp.Text))
        {
            case true:
                agentEdit.Email = mailComp.Text;
                break;
            case false:
                var msg = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Email имеет неверный формат");
                msg.ShowAsync();
                result = false;
                break;
        }

        if (result)
        {
            agentEdit.Name = nameComp.Text;
            agentEdit.Address = addressComp.Text;
            if (agentEdit.IdPriorityNavigation != null)
            {
                if (prChanged && agentEdit.IdPriorityNavigation.NamePr != priorAgCmb.SelectedValue)
                {
                    AgPriorChange priorChange = new AgPriorChange();
                    if (Helper.myprojContext.AgPriorChanges.Count() > 0)
                    {
                        priorChange.IdPriorChange = Helper.myprojContext.AgPriorChanges.OrderBy(x => x.IdPriorChange).LastOrDefault().IdPriorChange + 1;
                    }
                    else
                    {
                        priorChange.IdPriorChange = 1;
                    }
                    priorChange.IdNewPrior = Helper.myprojContext.Priorities.Where(x => x.NamePr == priorAgCmb.SelectedValue.ToString()).FirstOrDefault().IdPriority;
                    priorChange.IdNewPriorNavigation = Helper.myprojContext.Priorities.Where(x => x.IdPriority == priorChange.IdNewPrior).FirstOrDefault();
                    priorChange.IdAgent = agentEdit.IdAgent;
                    Helper.myprojContext.AgPriorChanges.Add(priorChange);
                    Helper.myprojContext.SaveChanges();
                    agentEdit.IdPriority = Helper.myprojContext.Priorities.Where(x => x.NamePr == priorAgCmb.SelectedItem.ToString()).FirstOrDefault().IdPriority;
                    agentEdit.IdPriorityNavigation = Helper.myprojContext.Priorities.Where(x => x.IdPriority == agentEdit.IdPriority).FirstOrDefault();

                }
            }
            else
            {
                agentEdit.IdPriority = Helper.myprojContext.Priorities.Where(x => x.NamePr == priorAgCmb.SelectedItem.ToString()).FirstOrDefault().IdPriority;
                agentEdit.IdPriorityNavigation = Helper.myprojContext.Priorities.Where(x => x.IdPriority == agentEdit.IdPriority).FirstOrDefault();
            }
            agentEdit.Phone = phoneComp.Text;
            agentEdit.IdType = Helper.myprojContext.Types.Where(x => x.NameType == typeAgCmb.SelectedItem.ToString()).FirstOrDefault().IdType;
            agentEdit.IdTypeNavigation = Helper.myprojContext.Types.Where(x => x.IdType == agentEdit.IdType).FirstOrDefault();
            if (filename != null)
            {
                File.Delete(agentEdit.Logo);
                agentEdit.Logo = filename;
                File.Move(path, destPath + "\\" + filename);
            }
            if (agentEdit.IdAgent != 0)
            {
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Name = agentEdit.Name;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().IdType = agentEdit.IdType;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().IdTypeNavigation = agentEdit.IdTypeNavigation;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Inn = agentEdit.Inn;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Sales = agentEdit.Sales;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Address = agentEdit.Address;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Logo = agentEdit.Logo;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Director = agentEdit.Director;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Kpp = agentEdit.Kpp;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Email = agentEdit.Email;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().IdPriority = agentEdit.IdPriority;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().IdPriorityNavigation = agentEdit.IdPriorityNavigation;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Phone = agentEdit.Phone;
                Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault().Sales = agentEdit.Sales;

            }
            else
            {
                //agentEdit.IdAgent = Helper.myprojContext.Agents.OrderBy(x => x.IdAgent).LastOrDefault().IdAgent + 1;
                Helper.myprojContext.Agents.Add(agentEdit);
                
            }
           
            Helper.myprojContext.SaveChanges();
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();

        }


    }
    private bool checkMail(string mail)
    {
        bool res = true;
        foreach (char s in mail)
        {
            if (mail.StartsWith("@") || mail.StartsWith(".") || !Char.IsLetter(s) && !Char.IsDigit(s) && !mail.Contains("@") && mail.Count(f => (f.ToString() == "@")) != 1 && mail.Count(f => (f.ToString() == ".")) != 1)
            {
                res = false;
            }
        }
        return res;
    }
    private bool checkFio(string fio)
    {
        bool res = true;
        foreach (char s in fio)
        {
            if (!Char.IsLetter(s) && !Char.IsWhiteSpace(s) && s.ToString() != "-")
            {
                res = false;
            }
        }
        return res;
    }
    private bool checkInn(string inn)
    {
        bool res = true;
        foreach (char s in inn)
        {
            if (!Char.IsDigit(s) && !Char.IsWhiteSpace(s) && s.ToString() != "-")
            {
                res = false;
            }
        }
        if (inn.Length != 10)
        {
            res = false;
        }
        return res;
    }
    private bool checKpp(string kpp)
    {
        bool res = true;
        foreach (char s in kpp)
        {
            if (!Char.IsDigit(s) && !Char.IsWhiteSpace(s) && s.ToString() != "-")
            {
                res = false;
            }
        }
        if (kpp.Length != 9)
        {
            res = false;
        }
        return res;
    }
    private  async void Img_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        OpenFileDialog openFile = new OpenFileDialog();
        var result = await openFile.ShowAsync(this);
        if (result == null) return;
        path = string.Join("", result);
        resultPhoto = result.ToString();
        if (result != null)
        {
            bitmapToBind = new Bitmap(path);
        }
        img.Source = bitmapToBind;
        filename = Path.GetFileName(path);
        destPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Assets";
    }
    private void PriorChanged_SelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        prChanged = true;

    }
        private  void AddReal_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (prodCmb.SelectedIndex==-1)
        {
            var msg = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Проверьте выбрали ли вы продукт");
            msg.ShowAsync();
        }
        else
        {
            bool res = true;
            foreach (char s in amountReal.Text)
            {
                if (!Char.IsDigit(s) || amountReal.Text.StartsWith("0"))
                {
                    res = false;
                    var msg = MessageBoxManager.GetMessageBoxStandard("Ошибка", "Проверьте правильно ли вы ввели количество");
                    msg.ShowAsync();
                }
            }
            if (res)
            {
                Sale sale = new Sale();
                sale.IdProduct = Helper.myprojContext.Products.Where(x => x.NameProduct == prodCmb.SelectedItem).FirstOrDefault().IdProduct;
                sale.Amount = Convert.ToInt32(amountReal.Text);
                sale.IdAgent = agentEdit.IdAgent;
                sale.IdAgentNavigation = Helper.myprojContext.Agents.Where(x => x.IdAgent == agentEdit.IdAgent).FirstOrDefault();
                sale.Date = DateOnly.FromDateTime(DateTime.Now.Date.Date);
                sale.IdProductNavigation = Helper.myprojContext.Products.Where(x => x.IdProduct == sale.IdProduct).FirstOrDefault();
                agentEdit.Sales.Add(sale);
                salesList.ItemsSource = agentEdit.Sales.ToList();
            }
        }
        
    }
    private void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int ind = (int)((sender as Button)!).Tag!;
        Sale sale = agentEdit.Sales.Where(x => x.IdSale == ind).FirstOrDefault();
        agentEdit.Sales.Remove(sale);
        salesList.ItemsSource = agentEdit.Sales.ToList();
    }
    private  void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow main = new MainWindow();
        main.Show();
        this.Close();
    }
}