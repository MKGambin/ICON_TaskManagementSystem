import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import axios from "axios";

const TaskItemList = () => {
    const { userId } = useParams();
    const [taskitems, setTaskItems] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        axios
            .get(`/api/taskitems?userIdentifier=${userId}`)
            .then((res) => {
                //alert(JSON.stringify(res));
                setTaskItems(res.data);
                setLoading(false);
            })
            .catch((err) => {
                //alert(JSON.stringify(err));
                console.error(err);
                setError("Failed to load taskitems.");
                setLoading(false);
            });
    }, []);

    const handleDelete = async (id) => {
        try {
            await axios.delete(`/api/taskitems/${userId}/${id}`);
            setTaskItems(taskitems.filter((x) => x.identifier !== id));
        } catch (err) {
            //alert(JSON.stringify(err));
            console.error(err);
            alert("Failed to delete taskitem.");
        }
    };

    if (loading) return <p>Loading taskitems...</p>;
    if (error) return <p>{error}</p>;

    const thTdStyle = { border: "1px solid #FFF", padding: "0.5rem", textAlign: "left", };

    return (
        <div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1rem" }}>
                <h3>Task Items</h3>
                <div>
                    <button onClick={() => window.location.href = `/users`} style={{ marginRight: "0.5rem" }}>
                        Back
                    </button>
                    <button onClick={() => window.location.href = `/taskitems/create/${userId}`}>
                        Create
                    </button>
                </div>
            </div>

            <hr />
            <table style={{ width: "100%", borderCollapse: "collapse", }}>
                <thead>
                    <tr>
                        <th style={thTdStyle}>Name</th>
                        <th style={thTdStyle}>Description</th>
                        <th style={thTdStyle}>TaskItemStatus</th>
                        <th style={thTdStyle}>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {taskitems.length === 0 ? (
                        <tr>
                            <td colSpan={2} style={{ textAlign: "center" }}>
                                No taskitems available.
                            </td>
                        </tr>
                    ) : (
                        taskitems.map((taskitem) => (
                            <tr key={taskitem.identifier}>
                                <td style={thTdStyle}>{taskitem.name}</td>
                                <td style={thTdStyle}>{taskitem.description}</td>
                                <td style={thTdStyle}>{taskitem.taskItemStatusText}</td>
                                <td style={thTdStyle}>
                                    <button onClick={() => window.location.href = `/taskitems/update/${userId}/${taskitem.identifier}`} style={{ marginRight: "0.5rem" }}>
                                        Edit
                                    </button>
                                    <button onClick={() => handleDelete(taskitem.identifier)}>
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default TaskItemList;
