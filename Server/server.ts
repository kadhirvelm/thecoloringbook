import express, { Request, Response } from "express";
import bodyParser from "body-parser";
import io from "socket.io";

const app = express();
const port = 3000;

app.use(bodyParser.json());

app.get("/", (req: Request, res: Response) => {
    res.json({
        message: "Received",
        version: "1.0.0"
    });
});

const server = app.listen(port, "192.168.0.108", (error: any) => {
    if (error) {
        console.error(error);
    }
    console.log(`Server running from port: ${port}`);
});

interface IMovement {
    vector: string;
}

const sockets = io(server);
sockets.on("connection", (socket) => {
    console.log("SOCKET CONNECTED", socket.id);

    socket.on("movement", (data: IMovement) => {
        socket.emit("movement_2", data);
    })
});
