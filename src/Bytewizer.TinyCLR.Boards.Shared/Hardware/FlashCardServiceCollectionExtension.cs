using System;

using Bytewizer.TinyCLR.DependencyInjection;

using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Storage;

namespace Bytewizer.TinyCLR.Boards
{
    public static class FlashCardServiceCollectionExtension
    {
        public static IServiceCollection AddFlashCard(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException();
            }

            var controller = StorageController.FromName(FEZDuino.StorageController.SdCard);

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(StorageController),
                    controller
                ));

            return services;
        }
    }
}