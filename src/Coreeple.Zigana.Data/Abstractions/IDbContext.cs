﻿using System.Data;

namespace Coreeple.Zigana.Data.Abstractions;
public interface IDbContext
{
    IDbConnection CreateConnection();
    void Migration();
}