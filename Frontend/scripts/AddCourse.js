let counter = 1;

function AddContent() {
  const type = document.getElementById("content-type").value;
  if (type === "lesson") AddLesson();
  else if (type === "quiz") AddQuiz();
  counter++;
}

// Reindex all course content (keeps everything consistent)
function updateIds() {
  const content = document.getElementById("course-content");
  [...content.children].forEach((child, index) => {
    const n = index + 1;
    child.dataset.index = n;
    child.id = `item-${n}`;
    const label = child.querySelector("span");
    if (label) {
      if (child.classList.contains("lesson")) label.textContent = `Lesson ${n} `;
      else if (child.classList.contains("quiz")) label.textContent = `Quiz ${n} `;
    }
  });
}

// Create a "remove" button with proper event logic
function createRemoveButton(item) {
  const removeBtn = document.createElement("button");
  removeBtn.textContent = "❌";
  removeBtn.title = "Remove this item";

  removeBtn.addEventListener("click", () => {
    const content = document.getElementById("course-content");
    content.removeChild(item);
    updateIds(); // refresh indices and labels
  });

  return removeBtn;
}

function AddLesson() {
  const content = document.getElementById("course-content");

  const item = document.createElement("div");
  item.className = "lesson";
  item.dataset.index = counter;
  item.id = `item-${counter}`;

  const label = document.createElement("span");
  label.textContent = `Lesson ${counter} `;

  const upBtn = document.createElement("button");
  upBtn.textContent = "⬆️";
  upBtn.addEventListener("click", () => {
    if (item.previousElementSibling) {
      content.insertBefore(item, item.previousElementSibling);
      updateIds();
    }
  });

  const downBtn = document.createElement("button");
  downBtn.textContent = "⬇️";
  downBtn.addEventListener("click", () => {
    if (item.nextElementSibling) {
      content.insertBefore(item.nextElementSibling, item);
      updateIds();
    }
  });

  const removeBtn = createRemoveButton(item);

  item.append(label, upBtn, downBtn, removeBtn);
  content.appendChild(item);
  updateIds();
}

function AddQuiz() {
  const content = document.getElementById("course-content");

  const item = document.createElement("div");
  item.className = "quiz";
  item.dataset.index = counter;
  item.id = `item-${counter}`;

  const label = document.createElement("span");
  label.textContent = `Quiz ${counter} `;

  const upBtn = document.createElement("button");
  upBtn.textContent = "⬆️";
  upBtn.addEventListener("click", () => {
    if (item.previousElementSibling) {
      content.insertBefore(item, item.previousElementSibling);
      updateIds();
    }
  });

  const downBtn = document.createElement("button");
  downBtn.textContent = "⬇️";
  downBtn.addEventListener("click", () => {
    if (item.nextElementSibling) {
      content.insertBefore(item.nextElementSibling, item);
      updateIds();
    }
  });

  const removeBtn = createRemoveButton(item);

  item.append(label, upBtn, downBtn, removeBtn);
  content.appendChild(item);
  updateIds();
}
