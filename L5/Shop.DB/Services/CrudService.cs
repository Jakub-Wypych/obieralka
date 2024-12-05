﻿using Microsoft.EntityFrameworkCore;
using Shared.Models.Dto;
using Shared.Services;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shop.DB.Services
{
    public class CrudService<T, TKey> : ICrudService<T, TKey> where T : class
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public CrudService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        protected ServiceReponse<TResponse> HandleException<TResponse>(Exception ex)
        {
            return new ServiceReponse<TResponse>
            {
                Success = false,
                Message = ex.Message,
                Data = default
            };
        }
        public virtual async Task<ServiceReponse<IEnumerable<T>>> GetAllAsync()
        {
            try
            {
                var data = await _dbSet.ToListAsync();
                return new ServiceReponse<IEnumerable<T>>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<T>>(ex);
            }
        }

        public virtual async Task<ServiceReponse<IEnumerable<T>>> GetAllAsyncs(ProductDto filterData)
        {
            try
            {
                var data = await _dbSet.ToListAsync();

                ObservableCollection<ProductDto>? products = new ObservableCollection<ProductDto>((IEnumerable<ProductDto>)data);

                var filteredProducts = products.AsQueryable();

                if (!string.IsNullOrEmpty(filterData.Name))
                {
                    filteredProducts = filteredProducts.Where(p => p.Name.Contains(filterData.Name));
                }

                if (filterData.Price.HasValue)
                {
                    filteredProducts = filteredProducts.Where(p => p.Price == filterData.Price.Value);
                }

                // Filtracja po CategoryId, jeśli wartość jest podana w filterData
                if (filterData.CategoryId.HasValue)
                {
                    filteredProducts = filteredProducts.Where(p => p.CategoryId == filterData.CategoryId.Value);
                }

                // Filtracja po StockId, jeśli wartość jest podana w filterData
                if (filterData.StockId.HasValue)
                {
                    filteredProducts = filteredProducts.Where(p => p.StockId == filterData.StockId.Value);
                }

                var finalData = filteredProducts.Cast<T>().ToList();

                return new ServiceReponse<IEnumerable<T>>
                {
                    Data = finalData,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<T>>(ex);
            }
        }


        public virtual async Task<ServiceReponse<T>> GetByIdAsync(TKey id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return new ServiceReponse<T>
                    {
                        Success = false,
                        Message = "Entity not found."
                    };
                }

                return new ServiceReponse<T>
                {
                    Data = entity,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        public virtual async Task<ServiceReponse<T>> CreateAsync(T entity)
        {
            try
            {
                _dbSet.Add(entity);
                await _dbContext.SaveChangesAsync();
                return new ServiceReponse<T>
                {
                    Data = entity,
                    Success = true,
                    Message = "Entity created successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        public virtual async Task<ServiceReponse<T>> UpdateAsync(TKey? id, T entity)
        {
            try
            {
                var existingEntity = await _dbSet.FindAsync(id);
                if (existingEntity == null)
                {
                    return new ServiceReponse<T>
                    {
                        Success = false,
                        Message = "Entity not found."
                    };
                }

                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _dbContext.SaveChangesAsync();
                return new ServiceReponse<T>
                {
                    Data = entity,
                    Success = true,
                    Message = "Entity updated successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<T>(ex);
            }
        }

        public virtual async Task<ServiceReponse<bool>> DeleteAsync(TKey? id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    return new ServiceReponse<bool>
                    {
                        Success = false,
                        Message = "Entity not found.",
                        Data = false
                    };
                }

                _dbSet.Remove(entity);
                await _dbContext.SaveChangesAsync();
                return new ServiceReponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "Entity deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<bool>(ex);
            }
        }

        public virtual async Task<ServiceReponse<bool>> DeleteAllAsync()
        {
            try
            {
                // Usunięcie wszystkich rekordów w tabeli
                _dbSet.RemoveRange(_dbSet);
                await _dbContext.SaveChangesAsync();

                return new ServiceReponse<bool>
                {
                    Data = true,
                    Success = true,
                    Message = "All entities deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return HandleException<bool>(ex);
            }
        }

    }
}
