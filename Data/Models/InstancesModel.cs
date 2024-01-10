using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Radzen;

public abstract class InstancesModels<T>
{
    private string left = "\"";
    private string right = "\"";


    public string GenerateAddQuery(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object cannot be null");

        }
        
        string query;
        string add = "insert into public." + left + obj.GetType().Name + right + " ";
        string valuesListe = " values (";
        string propertyListe = " (";
        foreach (var item in obj.GetType().GetProperties())
        {
            propertyListe += left + item.Name + right + ", ";
            valuesListe += item.GetValue(obj) + ", ";
        }

        propertyListe += ")";
        valuesListe += ")";
        query = add + propertyListe + valuesListe;

        return query;
    }

    public string GenerateDeleteQuery<TPrimaryKey>(T obj) where TPrimaryKey : Type
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object cannot be null");

        }

        string query;
        var primaryKey = obj.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttribute(typeof(TPrimaryKey)) != null);

        if (primaryKey == null)
        {
            throw new ArgumentNullException(nameof(obj), "Primary key not found");
        }
        else
        {
            var value = primaryKey.GetValue(obj);
            query = "delete from public." + left + obj.GetType().Name + right + " where " + left + primaryKey.Name + right + " = " + value;

        }

        return query;
    }

    public string GenerateGetQuery(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object cannot be null");

        }
        return "select * from public." + left + obj.GetType().Name + right;
    }

    public string GenerateUpdateQuery<TPrimaryKey>(T obj) where TPrimaryKey : Type
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object cannot be null");

        }

        string update = "Update public." + left + obj.GetType().Name + right + " set ";

        var primaryKey = obj.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttribute(typeof(TPrimaryKey)) != null);

        if (primaryKey == null)
        {
            throw new ArgumentNullException(nameof(obj), "Primary key not found");

        }

        foreach (var item in obj.GetType().GetProperties())
        {
            if (item.Name != primaryKey.Name)
            {
                update += left + item.Name + right + " = " + item.GetValue(obj) + ", ";
            }
        }

        update += " where " + left + primaryKey.Name + right + " = " + primaryKey.GetValue(obj);

        return update;
    }
}