import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";

const TaskItemForm = () => {
    const { userId, id } = useParams();
    const navigate = useNavigate();

    const [name, setName] = useState("");
    const [description, setDescription] = useState("");
    const [taskItemStatus, setTaskItemStatus] = useState(1);
    const [error, setError] = useState("");

    useEffect(() => {
        if (id) {
            axios
                .get(`/api/taskitems/update/${userId}/${id}`)
                .then((res) => {
                    setName(res.data.name);
                    setDescription(res.data.description);
                    setTaskItemStatus(res.data.taskItemStatus);
                })
                .catch((err) => setError("Failed to load taskitem."));
        }
    }, [id, userId]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (id) {
                await axios.put(`/api/taskitems/update`, { User: userId, Identifier: id, Name: name, Description: description, TaskItemStatus: taskItemStatus, });
            } else {
                await axios.post(`/api/taskitems/create`, { User: userId, Name: name, Description: description, TaskItemStatus: taskItemStatus, });
            }
            navigate(`/taskitems/${userId}`);
        } catch (err) {
            //alert(JSON.stringify(err));
            console.error(err);
            alert("An error occurred while saving the taskitem.");
        }
    };

    return (
        <div>
            <h3>{id ? "Edit Task Item" : "Create Task Item"}</h3>
            {error && <p style={{ color: "red" }}>{error}</p>}

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: "0.5rem" }}>
                    <label>Name:</label>
                    <input type="text" value={name} onChange={(e) => setName(e.target.value)} style={{ marginLeft: "0.5rem" }} required />
                </div>

                <div style={{ marginBottom: "0.5rem" }}>
                    <label>Description:</label>
                    <input type="text" value={description} onChange={(e) => setDescription(e.target.value)} style={{ marginLeft: "0.5rem" }} />
                </div>

                <div style={{ marginBottom: "0.5rem" }}>
                    <label>Status:</label>
                    <select value={taskItemStatus} onChange={(e) => setTaskItemStatus(Number(e.target.value))} style={{ marginLeft: "0.5rem" }}>
                        <option value={1}>Pending</option>
                        <option value={2}>InProgress</option>
                        <option value={3}>Completed</option>
                        <option value={4}>Cancelled</option>
                    </select>
                </div>

                <hr />
                <div style={{ display: "flex", gap: "0.5rem" }}>
                    <button type="button" onClick={() => navigate(`/taskitems/${userId}`)}>
                        Back
                    </button>
                    <button type="submit">
                        {id ? "Update" : "Create"}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default TaskItemForm;
