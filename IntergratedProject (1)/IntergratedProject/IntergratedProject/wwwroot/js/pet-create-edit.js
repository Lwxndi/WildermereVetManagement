$(document).ready(function () {
    function updateBreeds(speciesId, selectedBreedId = null) {
        const breedDropdown = $("#BreedDropdown, #BreedId").empty();

        if (!speciesId) {
            breedDropdown.append('<option value="">Select Species First</option>');
            return;
        }

        $.getJSON("/Pets/GetBreeds", { speciesId }, function (breeds) {
            if (!breeds || !breeds.length) {
                breedDropdown.append('<option value="">No breeds available</option>');
                return;
            }

            breedDropdown.append('<option value="">-- Select Breed --</option>');
            $.each(breeds, function (i, breed) {
                breedDropdown.append(`<option value="${breed.breedId || breed.BreedId}">${breed.name || breed.Name}</option>`);
            });

            if (selectedBreedId) {
                breedDropdown.val(selectedBreedId);
            }
        });
    }

    // Watch both possible IDs for species dropdown
    $("#SpeciesDropdown, #SpeciesId").change(function () {
        const speciesId = $(this).val();
        updateBreeds(speciesId);
    });

    // Initial load: prefill breeds if editing
    const initialSpeciesId = $("#SpeciesDropdown, #SpeciesId").val();
    if (initialSpeciesId) {
        updateBreeds(initialSpeciesId, $("#BreedDropdown, #BreedId").data("selected"));
    }
});
