﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using T1ELF0_HFT_2021222.Models;
using T1ELF0_HFT_2021222.Repository;

namespace T1ELF0_HFT_2021222.Logic
{
	public class CarLogic : ICarLogic
	{
		IRepository<Car> repo;
		IRepository<Brand> brandRepo;

		public CarLogic(IRepository<Car> repo, IRepository<Brand> brandRepo)
		{
			this.repo = repo;
			this.brandRepo = brandRepo;
		}

		public void Create(Car item)
		{
			try
			{
				Read(item.Id);
			}
			catch (Exception)
			{
				this.repo.Create(item);
				return;
			}
			throw new Exception("Id already in use");
		}

		public void Delete(int id)
		{
			try
			{
				Read(id);
			}
			catch (Exception)
			{
				return;
			}
			this.repo.Delete(id);
		}

		public Car Read(int id)
		{
			var q = this.repo.Read(id);
			if (q == null)
			{
				throw new Exception("Item not found");
			}

			return q;
		}

		public IQueryable<Car> ReadAll()
		{
			return this.repo.ReadAll();
		}

		public void Update(Car item)
		{
			if (Read(item.Id) == null)
			{
				throw new Exception("Item not found");
			}

			this.repo.Update(item);
		}

		public IEnumerable<BrandCount> CountByBrand()
		{
			var q = from car in repo.ReadAll()
					join brand in brandRepo.ReadAll()
					on car.BrandId equals brand.Id
					select new
					{
						Brand = brand.Name,
						Car = car.Id
					};
			var q2 = from item in q
					 group item by item.Brand into g
					 select new BrandCount()
					 {
						 Name = g.Key,
						 Count = q.Where(c => c.Brand == g.Key).Select(c => c.Car).Count()
					 };

			return q2;
		}
	}

	public class BrandCount
	{
		public string Name { get; set; }
		public int Count { get; set; }
	}
}
