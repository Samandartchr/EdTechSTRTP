const fileList = []; // local array to track uploaded files

document.getElementById("add-file-btn").addEventListener("click", () => {
  const input = document.createElement("input");
  input.type = "file";
  input.multiple = true;
  input.accept = "*/*";

  input.addEventListener("change", (event) => {
    const files = event.target.files;
    for (const file of files) {
      const fileObj = {
        name: file.name,
        file: file,
        description: ""
      };
      fileList.push(fileObj);
    }
    renderFileList();
  });

  input.click();
});

function renderFileList() {
  const container = document.getElementById("file-list");
  container.innerHTML = "";

  fileList.forEach((file, index) => {
    const fileDiv = document.createElement("div");
    fileDiv.className = "file-item";

    const nameSpan = document.createElement("span");
    nameSpan.textContent = file.name + " ";

    const descInput = document.createElement("input");
    descInput.type = "text";
    descInput.placeholder = "File description (optional)";
    descInput.value = file.description;
    descInput.addEventListener("input", (e) => {
      fileList[index].description = e.target.value;
    });

    // ✅ Create new buttons for each item (not shared)
    const upBtn = document.createElement("button");
    upBtn.textContent = "⬆️";
    upBtn.addEventListener("click", () => {
      if (index > 0) {
        [fileList[index - 1], fileList[index]] = [fileList[index], fileList[index - 1]];
        renderFileList();
      }
    });

    const downBtn = document.createElement("button");
    downBtn.textContent = "⬇️";
    downBtn.addEventListener("click", () => {
      if (index < fileList.length - 1) {
        [fileList[index + 1], fileList[index]] = [fileList[index], fileList[index + 1]];
        renderFileList();
      }
    });

    const removeBtn = document.createElement("button");
    removeBtn.textContent = "🗑️";
    removeBtn.addEventListener("click", () => {
      fileList.splice(index, 1);
      renderFileList();
    });

    fileDiv.append(nameSpan, descInput, upBtn, downBtn, removeBtn);
    container.appendChild(fileDiv);
  });
}

document.getElementById("save-lesson-btn").addEventListener("click", () => {
  const title = document.getElementById("lesson-title").value.trim();
  const description = document.getElementById("lesson-description").value.trim();

  if (!title) {
    alert("Title is required!");
    return;
  }

  const lessonData = {
    title,
    description,
    files: fileList.map(f => ({
      name: f.name,
      description: f.description
      // later: file URLs or IDs from backend
    }))
  };

  console.log("Lesson saved:", lessonData);
  alert("Lesson data logged to console for now!");
});
