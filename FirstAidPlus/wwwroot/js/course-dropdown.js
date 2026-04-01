document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('courseMenuSearch');
    const courseItems = document.querySelectorAll('.course-item');
    const categoryHeaders = document.querySelectorAll('.category-header');

    if (searchInput) {
        searchInput.addEventListener('input', function (e) {
            const term = e.target.value.toLowerCase().trim();
            console.log('Searching for:', term);
            let visibleCount = 0;

            courseItems.forEach(item => {
                const title = item.textContent.toLowerCase();
                if (title.includes(term)) {
                    item.style.display = 'block';
                    visibleCount++;
                } else {
                    item.style.display = 'none';
                }
            });

            // Hide/Show category headers based on whether they have visible children
            categoryHeaders.forEach(header => {
                const category = header.getAttribute('data-category');
                const hasVisibleCourses = Array.from(courseItems).some(item => 
                    item.getAttribute('data-category') === category && !item.classList.contains('d-none')
                );

                if (hasVisibleCourses || term === "") {
                    header.classList.remove('d-none');
                } else {
                    header.classList.add('d-none');
                }
            });

            // Special handling for "Khám phá tất cả" and dividers
            const allLink = document.querySelector('.course-all-link');
            const divider = document.querySelector('.course-divider');
            if (term !== "") {
                allLink?.classList.add('d-none');
                divider?.classList.add('d-none');
            } else {
                allLink?.classList.remove('d-none');
                divider?.classList.remove('d-none');
            }
        });

        // Prevent dropdown from closing when clicking the search input
        searchInput.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    }
});
